using ExamiNation.Domain.Entities.Test;
using System.Linq.Expressions;

namespace ExamiNation.Domain.Interfaces.Test
{
    public interface IQuestionRepository
    {
        Task<Question?> GetByIdAsync(Guid id, bool asNoTracking = true, params Expression<Func<Question, object>>[] includes);

        Task<IEnumerable<Question>> GetQuestionsAsync(Expression<Func<Question, bool>>? filter = null, bool asNoTracking = true,params Expression<Func<Question, object>>[] includes);

        Task<Question?> FindFirstQuestionAsync(Expression<Func<Question, bool>> filter, bool asNoTracking = true,params Expression<Func<Question, object>>[] includes);

        Task<Question> AddAsync(Question role);

        Task<Question?> UpdateAsync(Question role);

        Task<Question?> DeleteAsync(Guid id);
    }
}
