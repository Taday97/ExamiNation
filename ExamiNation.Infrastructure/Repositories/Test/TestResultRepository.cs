using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExamiNation.Infrastructure.Repositories.Test
{
    public class TestResultRepository : ITestResultRepository
    {
        private readonly AppDbContext _context;

        public TestResultRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TestResult?> GetByIdAsync(Guid id, bool asNoTracking = true)
        {
            var query = _context.TestResults.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<TestResult?> GetByIdWithAnswersAsync(Guid id, bool asNoTracking = true)
        {
            var query = _context.TestResults.Include("Answer").AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<TestResult>> GetTestResultsAsync(Expression<Func<TestResult, bool>>? filter = null, bool asNoTracking = true)
        {
            IQueryable<TestResult> query = _context.TestResults;

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

        public async Task<TestResult?> FindFirstTestResultAsync(Expression<Func<TestResult, bool>> filter, bool asNoTracking = true)
        {
            var query = _context.TestResults.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<TestResult> AddAsync(TestResult option)
        {
            await _context.TestResults.AddAsync(option);
            await _context.SaveChangesAsync();
            return option;
        }

        public async Task<TestResult?> UpdateAsync(TestResult option)
        {
            var existingTestResult = await _context.TestResults.FindAsync(option.Id);
            if (existingTestResult == null)
            {
                return null;
            }

            _context.Entry(existingTestResult).CurrentValues.SetValues(option);
            await _context.SaveChangesAsync();
            return existingTestResult;
        }

        public async Task<TestResult?> DeleteAsync(Guid id)
        {
            var option = await _context.TestResults.FindAsync(id);
            if (option == null)
            {
                return null;
            }

            _context.TestResults.Remove(option);
            await _context.SaveChangesAsync();
            return option;
        }
    }
}
