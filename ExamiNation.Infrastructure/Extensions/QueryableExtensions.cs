using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExamiNation.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, Dictionary<string, string> filters)
        {
            foreach (var filter in filters)
            {
                var property = typeof(T).GetProperty(filter.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null) continue;

                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyAccess = Expression.Property(parameter, property);
                Expression filterCondition = null;

                if (property.PropertyType.IsEnum)
                {
                    var enumType = property.PropertyType;

                    if (Enum.TryParse(enumType, filter.Value, ignoreCase: true, out object? parsedEnum))
                    {
                        var searchConstant = Expression.Constant(parsedEnum);
                        filterCondition = Expression.Equal(propertyAccess, searchConstant);
                    }
                    else if (int.TryParse(filter.Value, out int enumNumericValue))
                    {
                        var enumValue = Enum.ToObject(enumType, enumNumericValue);
                        var searchConstant = Expression.Constant(enumValue);
                        filterCondition = Expression.Equal(propertyAccess, searchConstant);
                    }
                }

                if (property.PropertyType == typeof(string))
                {
                    var searchConstant = Expression.Constant(filter.Value);
                    filterCondition = Expression.Call(propertyAccess, "Contains", null, searchConstant);
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    if (Guid.TryParse(filter.Value, out var guidResult))
                    {
                        var searchConstant = Expression.Constant(guidResult);
                        filterCondition = Expression.Equal(propertyAccess, searchConstant);
                    }
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    if (DateTime.TryParse(filter.Value, out var dateResult))
                    {
                        var searchConstant = Expression.Constant(dateResult);
                        filterCondition = Expression.Equal(propertyAccess, searchConstant);
                    }
                }
                else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(long) || property.PropertyType == typeof(float) || property.PropertyType == typeof(double))
                {
                    if (double.TryParse(filter.Value, out var numberResult))
                    {
                        var searchConstant = Expression.Constant(Convert.ChangeType(numberResult, property.PropertyType));
                        filterCondition = Expression.Equal(propertyAccess, searchConstant);
                    }
                }

                if (filterCondition != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(filterCondition, parameter);
                    query = query.Where(lambda);
                }
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
