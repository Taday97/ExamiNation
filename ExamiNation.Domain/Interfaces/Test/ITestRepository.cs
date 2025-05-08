using ExamiNation.Domain.Common;
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
    public interface ITestRepository : IGenericRepository<TestEntity>
    {
        
    }
}
