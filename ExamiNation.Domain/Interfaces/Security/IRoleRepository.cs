using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Security;
using System.Linq.Expressions;

namespace ExamiNation.Domain.Interfaces.Security
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid id, bool asNoTracking = true);

        Task<IEnumerable<Role>> GetRolesAsync(Expression<Func<Role, bool>> filter = null, bool asNoTracking = true);

        Task<Role?> FindFirstRoleAsync(Expression<Func<Role, bool>> filter, bool asNoTracking = true);

        Task<Role> AddAsync(Role role);

        Task<Role?> UpdateAsync(Role role);

        Task<Role?> DeleteAsync(Guid id);
        Task<(IEnumerable<Role> Items, int TotalCount, Dictionary<Guid, List<string?>>)> GetPagedWithCountAsync(PagedQueryOptions<Role> options);
    }
}
