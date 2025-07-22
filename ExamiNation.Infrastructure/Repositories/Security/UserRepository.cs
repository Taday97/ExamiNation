using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Security;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Infrastructure.Data;
using ExamiNation.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace ExamiNation.Infrastructure.Repositories.Security
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<ApplicationUser> GetByIdAsync(string id, bool asNoTracking = true)
        {
            if (!Guid.TryParse(id, out Guid parsedId))
            {
                return null;
            }

            IQueryable<ApplicationUser> query = _context.Users;

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(l => l.Id == parsedId);
        }


        public async Task<ApplicationUser> AddAsync(ApplicationUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync(bool asNoTracking = true)
        {
            IQueryable<ApplicationUser> query = _context.Users;

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public async Task<ApplicationUser> UpdateAsync(ApplicationUser user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with id {user.Id} not found.");
            }

            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<ApplicationUser> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id {id} not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> AssignRolesToUserAsync(ApplicationUser user, IEnumerable<Role> roles)
        {
            try
            {
                foreach (var role in roles)
                {
                    if (!await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<bool> RemoveRolesFromUserAsync(ApplicationUser user, IEnumerable<Role> roles)
        {
            try
            {
                foreach (var role in roles)
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync(Expression<Func<ApplicationUser, bool>> filter = null, bool asNoTracking = true)
        {
            IQueryable<ApplicationUser> query = _userManager.Users;

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

        public async Task<ApplicationUser?> FindFirstUserAsync(Expression<Func<ApplicationUser, bool>> filter, bool asNoTracking = true)
        {
            IQueryable<ApplicationUser> query = _userManager.Users;

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<(IEnumerable<ApplicationUser> Items, int TotalCount, Dictionary<Guid, List<string?>>)> GetPagedWithCountAsync(PagedQueryOptions<ApplicationUser> options)
        {
            IQueryable<ApplicationUser> query = _userManager.Users;

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
                var roleIds = rolesFilter
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => Guid.TryParse(r.Trim(), out var id) ? id : (Guid?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (roleIds.Any())
                {
                    var userIdsWithRole = await _context.UserRoles
                        .Where(ur => roleIds.Contains(ur.RoleId))
                        .Select(ur => ur.UserId)
                        .Distinct()
                        .ToListAsync();

                    query = query.Where(u => userIdsWithRole.Contains(u.Id));
                }
            }
            else if(options?.Filters != null && options.Filters.Any())
            {
                query = query.ApplyFilters(options.Filters);
            }



            var totalCount = await query.CountAsync();


            query = query
                .Skip(((options.PageNumber ?? 1) - 1) * (options.PageSize ?? int.MaxValue))
                .Take(options.PageSize ?? int.MaxValue);

            var users = await query.ToListAsync();

            var userIds = users.Select(u => u.Id).ToList();

            var userRolesRaw = await _context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .ToListAsync();

           

            var roles = await _context.Roles.ToListAsync();

            var userRolesDict = userRolesRaw
                .GroupBy(ur => ur.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(ur => roles.First(r => r.Id == ur.RoleId).Name).ToList()
                );

            return (users, totalCount, userRolesDict);
        }
    }
}