using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExamiNation.Infrastructure.Repositories.Test
{
    public class OptionRepository : IOptionRepository
    {
        private readonly AppDbContext _context;

        public OptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Option?> GetByIdAsync(Guid id, bool asNoTracking = true)
        {
            var query = _context.Options.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Option>> GetOptionsAsync(Expression<Func<Option, bool>>? filter = null, bool asNoTracking = true)
        {
            IQueryable<Option> query = _context.Options;

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

        public async Task<Option?> FindFirstOptionAsync(Expression<Func<Option, bool>> filter, bool asNoTracking = true)
        {
            var query = _context.Options.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<Option> AddAsync(Option role)
        {
            await _context.Options.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<Option?> UpdateAsync(Option role)
        {
            var existingOption = await _context.Options.FindAsync(role.Id);
            if (existingOption == null)
            {
                return null;
            }

            _context.Entry(existingOption).CurrentValues.SetValues(role);
            await _context.SaveChangesAsync();
            return existingOption;
        }

        public async Task<Option?> DeleteAsync(Guid id)
        {
            var role = await _context.Options.FindAsync(id);
            if (role == null)
            {
                return null;
            }

            _context.Options.Remove(role);
            await _context.SaveChangesAsync();
            return role;
        }
    }
}
