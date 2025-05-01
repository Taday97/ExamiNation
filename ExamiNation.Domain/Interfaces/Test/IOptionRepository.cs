using ExamiNation.Domain.Entities.Test;
using System.Linq.Expressions;

namespace ExamiNation.Domain.Interfaces.Test
{
    public interface IOptionRepository
    {
        Task<Option?> GetByIdAsync(Guid id, bool asNoTracking = true);

        Task<IEnumerable<Option>> GetOptionsAsync(Expression<Func<Option, bool>> filter = null, bool asNoTracking = true);

        Task<Option?> FindFirstOptionAsync(Expression<Func<Option, bool>> filter, bool asNoTracking = true);

        Task<Option> AddAsync(Option role);

        Task<Option?> UpdateAsync(Option role);

        Task<Option?> DeleteAsync(Guid id);
    }
}
