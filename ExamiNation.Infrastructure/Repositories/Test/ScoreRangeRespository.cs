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
    public class ScoreRangeRespository : GenericRepository<ScoreRange>, IScoreRangeRepository
    {
        public ScoreRangeRespository(AppDbContext context) : base(context) { }
    }
}