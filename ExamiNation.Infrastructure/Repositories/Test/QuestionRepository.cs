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

        public async Task<Question?> GetByIdAsync( Guid id,bool asNoTracking = true, params Expression<Func<Question, object>>[] includes)
        {
            var query = _context.Questions.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(q => q.Id == id);
        }



        public async Task<IEnumerable<Question>> GetQuestionsAsync(
        Expression<Func<Question, bool>>? filter = null,
        bool asNoTracking = true,
        params Expression<Func<Question, object>>[] includes) 
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

            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.ToListAsync();
        }


        public async Task<Question?> FindFirstQuestionAsync(Expression<Func<Question, bool>> filter,bool asNoTracking = true,
        params Expression<Func<Question, object>>[] includes) 
        {
            var query = _context.Questions.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            query = query.Where(filter);

            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync();
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
