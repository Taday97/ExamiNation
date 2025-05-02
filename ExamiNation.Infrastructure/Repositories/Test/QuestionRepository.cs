using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExamiNation.Infrastructure.Repositories.Test
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext _context;

        public QuestionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Question?> GetByIdAsync(Guid id, bool asNoTracking = true)
        {
            var query = _context.Questions.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Question>> GetQuestionsAsync(Expression<Func<Question, bool>>? filter = null, bool asNoTracking = true)
        {
            IQueryable<Question> query = _context.Questions;

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

        public async Task<Question?> FindFirstQuestionAsync(Expression<Func<Question, bool>> filter, bool asNoTracking = true)
        {
            var query = _context.Questions.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<Question> AddAsync(Question option)
        {
            await _context.Questions.AddAsync(option);
            await _context.SaveChangesAsync();
            return option;
        }

        public async Task<Question?> UpdateAsync(Question option)
        {
            var existingQuestion = await _context.Questions.FindAsync(option.Id);
            if (existingQuestion == null)
            {
                return null;
            }

            _context.Entry(existingQuestion).CurrentValues.SetValues(option);
            await _context.SaveChangesAsync();
            return existingQuestion;
        }

        public async Task<Question?> DeleteAsync(Guid id)
        {
            var option = await _context.Questions.FindAsync(id);
            if (option == null)
            {
                return null;
            }

            _context.Questions.Remove(option);
            await _context.SaveChangesAsync();
            return option;
        }
    }
}
