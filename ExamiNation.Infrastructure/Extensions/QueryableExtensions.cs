﻿using System.Linq.Expressions;
using System.Reflection;

namespace ExamiNation.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, Dictionary<string, string> filters)
        {
            int appliedFilters = 0;

            foreach (var filter in filters)
            {
                var property = typeof(T).GetProperty(filter.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null) continue;

                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyAccess = Expression.Property(parameter, property);
                Expression? filterCondition = null;

                Type propertyType = property.PropertyType;
                Type underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

                if (underlyingType.IsEnum)
                {
                    if (Enum.TryParse(underlyingType, filter.Value, ignoreCase: true, out object? parsedEnum))
                    {
                        var searchConstant = Expression.Constant(parsedEnum, propertyType);
                        filterCondition = Expression.Equal(propertyAccess, searchConstant);
                    }
                    else if (int.TryParse(filter.Value, out int enumNumericValue))
                    {
                        var enumValue = Enum.ToObject(underlyingType, enumNumericValue);
                        var searchConstant = Expression.Constant(enumValue, propertyType);
                        filterCondition = Expression.Equal(propertyAccess, searchConstant);
                    }
                }
                else if (underlyingType == typeof(string))
                {
                    var searchConstant = Expression.Constant(filter.Value, typeof(string));
                    filterCondition = Expression.Call(propertyAccess, "Contains", null, searchConstant);
                }
                else if (underlyingType == typeof(Guid))
                {
                    if (Guid.TryParse(filter.Value, out var guidResult))
                    {
                        var searchConstant = Expression.Constant(guidResult, propertyType);
                        filterCondition = Expression.Equal(propertyAccess, searchConstant);
                    }
                }
                else if (underlyingType == typeof(DateTime))
                {
                    if (DateTime.TryParse(filter.Value, out var dateResult))
                    {
                        var searchConstant = Expression.Constant(dateResult, propertyType);
                        filterCondition = Expression.Equal(propertyAccess, searchConstant);
                    }
                }
                else if (new[] { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) }.Contains(underlyingType))
                {
                    if (double.TryParse(filter.Value, out var numberResult))
                    {
                        var convertedValue = Convert.ChangeType(numberResult, underlyingType);
                        var searchConstant = Expression.Constant(convertedValue, propertyType);
                        filterCondition = Expression.Equal(propertyAccess, searchConstant);
                    }
                }

                if (filterCondition != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(filterCondition, parameter);
                    query = query.Where(lambda);
                    appliedFilters++;
                }
            }

            if (filters.Count > 0 && appliedFilters == 0)
            {
                return query.Where(_ => false); 
            }

            return query;
        }



        public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, string? sortBy, bool descending)
        {
            if (string.IsNullOrWhiteSpace(sortBy)) return query;

            var property = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null) return query;

            var param = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(param, property);
            var lambda = Expression.Lambda(propertyAccess, param);

            var method = descending ? "OrderByDescending" : "OrderBy";

            var result = typeof(Queryable).GetMethods()
                .First(m => m.Name == method && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.PropertyType)
                .Invoke(null, new object[] { query, lambda });

            return (IQueryable<T>)result!;
        }
    }
}
