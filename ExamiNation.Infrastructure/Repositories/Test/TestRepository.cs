using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TestEntity = ExamiNation.Domain.Entities.Test.Test;

namespace ExamiNation.Infrastructure.Repositories.Test
{
    public class TestRepository : ITestRepository
    {
        private readonly AppDbContext _context;

        public TestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TestEntity?> GetByIdAsync(Guid id, bool asNoTracking = true)
        {
            var query = _context.Tests.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<TestEntity?> GetByIdWithQuestionsAsync(Guid id, bool asNoTracking = true)
        {
            var query = _context.Tests.Include("Questions").Include("Options").AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<TestEntity>> GetTestEntitysAsync(Expression<Func<TestEntity, bool>>? filter = null, bool asNoTracking = true)
        {
            IQueryable<TestEntity> query = _context.Tests;

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

        public async Task<TestEntity?> FindFirstTestEntityAsync(Expression<Func<TestEntity, bool>> filter, bool asNoTracking = true)
        {
            var query = _context.Tests.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<TestEntity> AddAsync(TestEntity testEntity)
        {
            await _context.Tests.AddAsync(testEntity);
            await _context.SaveChangesAsync();
            return testEntity;
        }

        public async Task<TestEntity?> UpdateAsync(TestEntity testEntity)
        {
            var existingTestEntity = await _context.Tests.FindAsync(testEntity.Id);
            if (existingTestEntity == null)
            {
                return null;
            }

            _context.Entry(existingTestEntity).CurrentValues.SetValues(testEntity);
            await _context.SaveChangesAsync();
            return existingTestEntity;
        }

        public async Task<TestEntity?> DeleteAsync(Guid id)
        {
            var testEntity = await _context.Tests.FindAsync(id);
            if (testEntity == null)
            {
                return null;
            }

            _context.Tests.Remove(testEntity);
            await _context.SaveChangesAsync();
            return testEntity;
        }
    }
}
