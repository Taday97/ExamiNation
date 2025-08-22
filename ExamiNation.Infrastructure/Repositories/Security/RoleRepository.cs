using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Security;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Infrastructure.Data;
using ExamiNation.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExamiNation.Infrastructure.Repositories.Security
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(Guid id, bool asNoTracking = true)
        {
            var query = _context.Roles.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Role>> GetRolesAsync(Expression<Func<Role, bool>>? filter = null, bool asNoTracking = true)
        {
            IQueryable<Role> query = _context.Roles;

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<Role?> FindFirstRoleAsync(Expression<Func<Role, bool>> filter, bool asNoTracking = true)
        {
            var query = _context.Roles.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<Role> AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<Role?> UpdateAsync(Role role)
        {
            var existingRole = await _context.Roles.FindAsync(role.Id);
            if (existingRole == null)
            {
                return null;
            }

            _context.Entry(existingRole).CurrentValues.SetValues(role);
            await _context.SaveChangesAsync();
            return existingRole;
        }

        public async Task<Role?> DeleteAsync(Guid id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return null;
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return role;
        }
        public async Task<(IEnumerable<Role> Items, int TotalCount, Dictionary<Guid, List<string?>>)> GetPagedWithCountAsync(PagedQueryOptions<Role> options)
        {
            IQueryable<Role> query = _context.Roles;

            if (options.AsNoTracking)
                query = query.AsNoTracking();

            if (options?.Includes != null)
            {
                foreach (var include in options.Includes)
                {
                    query = query.Include(include);
                }
            }

            if (options?.ThenIncludes != null)
            {
                foreach (var thenInclude in options.ThenIncludes)
                {
                    query = thenInclude(query);
                }
            }

            if (!string.IsNullOrEmpty(options?.SortBy))
            {
                query = query.ApplyOrdering(options.SortBy, options.SortDescending);
            }

            if (options?.Filters != null && options.Filters.TryGetValue("roles", out var rolesFilter) && !string.IsNullOrWhiteSpace(rolesFilter))
            {
                var roleNewIds = rolesFilter
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => Guid.TryParse(r.Trim(), out var id) ? id : (Guid?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (roleNewIds.Any())
                {
                    var userIdsWithRole = await _context.UserRoles
                        .Where(ur => roleNewIds.Contains(ur.RoleId))
                        .Select(ur => ur.UserId)
                        .Distinct()
                        .ToListAsync();

                    query = query.Where(r => userIdsWithRole.Contains(r.Id));
                }
            }
            else if (options?.Filters != null && options.Filters.Any())
            {
                query = query.ApplyFilters(options.Filters);
            }

            var totalCount = await query.CountAsync();

            query = query
                .Skip(((options.PageNumber ?? 1) - 1) * (options.PageSize ?? int.MaxValue))
                .Take(options.PageSize ?? int.MaxValue);

            var pagedRoles = await query.ToListAsync();
            var roleIds = pagedRoles.Select(r => r.Id).ToList();

            var userRolesRaw = await _context.UserRoles
                .Where(ur => roleIds.Contains(ur.RoleId))
                .ToListAsync();

            var userRolesDict = userRolesRaw
                .GroupBy(ur => ur.RoleId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(ur => ur.UserId.ToString()).ToList()
                );

            return (pagedRoles, totalCount, userRolesDict);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);

            return role != null;
        }
    }
}
