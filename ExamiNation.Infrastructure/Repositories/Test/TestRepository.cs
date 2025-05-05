using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TestEntity = ExamiNation.Domain.Entities.Test.Test;

namespace ExamiNation.Infrastructure.Repositories.Test
{
    public class TestRepository : GenericRepository<TestEntity>, ITestRepository
    {
        public TestRepository(AppDbContext context) : base(context) { }
    }
}