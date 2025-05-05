using ExamiNation.Domain.Common;
using System.Linq.Expressions;

namespace ExamiNation.Domain.Interfaces.Test
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? options = null);
        Task<T?> FindFirstAsync(Expression<Func<T, bool>> filter, bool asNoTracking = true, params Expression<Func<T, object>>[] includes);
        Task<T> AddAsync(T entity);
        Task<T?> UpdateAsync(T entity);
        Task<T?> DeleteAsync(Guid id);
    }

}
