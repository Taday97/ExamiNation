using ExamiNation.Domain.Entities.Test;
using System.Linq.Expressions;

namespace ExamiNation.Domain.Interfaces.Test
{
    public interface ITestResultRepository
    {
        Task<TestResult?> GetByIdAsync(Guid id, bool asNoTracking = true);

        Task<IEnumerable<TestResult>> GetTestResultsAsync(Expression<Func<TestResult, bool>> filter = null, bool asNoTracking = true);

        Task<TestResult?> FindFirstTestResultAsync(Expression<Func<TestResult, bool>> filter, bool asNoTracking = true);

        Task<TestResult> AddAsync(TestResult role);

        Task<TestResult?> UpdateAsync(TestResult role);

        Task<TestResult?> DeleteAsync(Guid id);
    }
}
