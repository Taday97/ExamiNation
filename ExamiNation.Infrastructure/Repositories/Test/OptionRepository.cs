using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExamiNation.Infrastructure.Repositories.Test
{
    public class OptionRepository : GenericRepository<Option>, IOptionRepository
    {
        public OptionRepository(AppDbContext context) : base(context) { }
    }
}