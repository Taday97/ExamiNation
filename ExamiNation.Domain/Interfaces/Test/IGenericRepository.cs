using ExamiNation.Domain.Common;
using System.Linq.Expressions;

namespace ExamiNation.Domain.Interfaces.Test
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? options = null);
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedWithCountAsync(PagedQueryOptions<T> options);
        Task<T?> FindFirstAsync(Expression<Func<T, bool>> filter, bool asNoTracking = true, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<T> AddAsync(T entity);
        Task<T?> UpdateAsync(T entity);
        Task<T?> DeleteAsync(Guid id);
    }

}
