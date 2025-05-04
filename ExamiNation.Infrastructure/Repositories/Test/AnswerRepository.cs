using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExamiNation.Infrastructure.Repositories.Test
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly AppDbContext _context;

        public AnswerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Answer?> GetByIdAsync(Guid id, bool asNoTracking = true)
        {
            var query = _context.Answeres.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Answer>> GetAnswersAsync(Expression<Func<Answer, bool>>? filter = null, bool asNoTracking = true)
        {
            IQueryable<Answer> query = _context.Answeres;

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

        public async Task<Answer?> FindFirstAnswerAsync(Expression<Func<Answer, bool>> filter, bool asNoTracking = true)
        {
            var query = _context.Answeres.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<Answer> AddAsync(Answer option)
        {
            await _context.Answeres.AddAsync(option);
            await _context.SaveChangesAsync();
            return option;
        }

        public async Task<Answer?> UpdateAsync(Answer option)
        {
            var existingAnswer = await _context.Answeres.FindAsync(option.Id);
            if (existingAnswer == null)
            {
                return null;
            }

            _context.Entry(existingAnswer).CurrentValues.SetValues(option);
            await _context.SaveChangesAsync();
            return existingAnswer;
        }

        public async Task<Answer?> DeleteAsync(Guid id)
        {
            var option = await _context.Answeres.FindAsync(id);
            if (option == null)
            {
                return null;
            }

            _context.Answeres.Remove(option);
            await _context.SaveChangesAsync();
            return option;
        }
    }
}
