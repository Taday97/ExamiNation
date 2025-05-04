using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Infrastructure.Repositories.Test
{
    public class ScoreRangeRespository : IScoreRangeRepository
    {
        private readonly AppDbContext _context;

        public ScoreRangeRespository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ScoreRange?> GetByIdAsync(Guid id, bool asNoTracking = true)
        {
            var query = _context.ScoreRanges.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<ScoreRange>> GetScoreRangesAsync(Expression<Func<ScoreRange, bool>>? filter = null, bool asNoTracking = true)
        {
            IQueryable<ScoreRange> query = _context.ScoreRanges;

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

        public async Task<ScoreRange?> FindFirstScoreRangeAsync(Expression<Func<ScoreRange, bool>> filter, bool asNoTracking = true)
        {
            var query = _context.ScoreRanges.AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<ScoreRange> AddAsync(ScoreRange scoreRange)
        {
            await _context.ScoreRanges.AddAsync(scoreRange);
            await _context.SaveChangesAsync();
            return scoreRange;
        }

        public async Task<ScoreRange?> UpdateAsync(ScoreRange scoreRange)
        {
            var existingScoreRange = await _context.ScoreRanges.FindAsync(scoreRange.Id);
            if (existingScoreRange == null)
            {
                return null;
            }

            _context.Entry(existingScoreRange).CurrentValues.SetValues(scoreRange);
            await _context.SaveChangesAsync();
            return existingScoreRange;
        }

        public async Task<ScoreRange?> DeleteAsync(Guid id)
        {
            var scoreRange = await _context.ScoreRanges.FindAsync(id);
            if (scoreRange == null)
            {
                return null;
            }

            _context.ScoreRanges.Remove(scoreRange);
            await _context.SaveChangesAsync();
            return scoreRange;
        }
    }
}
