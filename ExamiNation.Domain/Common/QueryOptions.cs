using System.Linq.Expressions;

namespace ExamiNation.Domain.Common
{
    public class QueryOptions<T>
    {
        public Expression<Func<T, bool>>? Filter { get; set; }
        public bool AsNoTracking { get; set; } = true;
        public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new();


    }
}
