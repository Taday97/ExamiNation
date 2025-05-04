using ExamiNation.Domain.Entities.Test;
using System.Linq.Expressions;

namespace ExamiNation.Domain.Interfaces.Test
{
    public interface IScoreRangeRepository
    {
        Task<ScoreRange?> GetByIdAsync(Guid id, bool asNoTracking = true);

        Task<IEnumerable<ScoreRange>> GetScoreRangesAsync(Expression<Func<ScoreRange, bool>> filter = null, bool asNoTracking = true);

        Task<ScoreRange?> FindFirstScoreRangeAsync(Expression<Func<ScoreRange, bool>> filter, bool asNoTracking = true);

        Task<ScoreRange> AddAsync(ScoreRange role);

        Task<ScoreRange?> UpdateAsync(ScoreRange role);

        Task<ScoreRange?> DeleteAsync(Guid id);
    }
}
