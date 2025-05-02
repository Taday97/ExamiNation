using ExamiNation.Domain.Entities.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TestEntity = ExamiNation.Domain.Entities.Test.Test;

namespace ExamiNation.Domain.Interfaces.Test
{
    public interface ITestRepository
    {
        Task<TestEntity?> GetByIdAsync(Guid id, bool asNoTracking = true);

        Task<IEnumerable<TestEntity>> GetTestEntitysAsync(Expression<Func<TestEntity, bool>> filter = null, bool asNoTracking = true);

        Task<TestEntity?> FindFirstTestEntityAsync(Expression<Func<TestEntity, bool>> filter, bool asNoTracking = true);

        Task<TestEntity> AddAsync(TestEntity role);

        Task<TestEntity?> UpdateAsync(TestEntity role);

        Task<TestEntity?> DeleteAsync(Guid id);
    }
}
