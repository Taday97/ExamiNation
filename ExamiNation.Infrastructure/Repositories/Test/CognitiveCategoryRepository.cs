using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;

namespace ExamiNation.Infrastructure.Repositories.Test
{
    public class CognitiveCategoryRepository : GenericRepository<CognitiveCategory>, ICognitiveCategoryRepository
    {
        public CognitiveCategoryRepository(AppDbContext context) : base(context) { }
    }
}