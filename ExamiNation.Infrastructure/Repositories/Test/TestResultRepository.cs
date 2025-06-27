using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExamiNation.Infrastructure.Repositories.Test
{
    public class TestResultRepository : GenericRepository<TestResult>, ITestResultRepository
    {
        public TestResultRepository(AppDbContext context) : base(context) { }



    }
}