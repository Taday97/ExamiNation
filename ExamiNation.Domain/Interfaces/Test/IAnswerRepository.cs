using ExamiNation.Domain.Entities.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Domain.Interfaces.Test
{
    public interface IAnswerRepository
    {
        Task<Answer?> GetByIdAsync(Guid id, bool asNoTracking = true);

        Task<IEnumerable<Answer>> GetAnswersAsync(Expression<Func<Answer, bool>> filter = null, bool asNoTracking = true);

        Task<Answer?> FindFirstAnswerAsync(Expression<Func<Answer, bool>> filter, bool asNoTracking = true);

        Task<Answer> AddAsync(Answer role);

        Task<Answer?> UpdateAsync(Answer role);

        Task<Answer?> DeleteAsync(Guid id);
    }
}
