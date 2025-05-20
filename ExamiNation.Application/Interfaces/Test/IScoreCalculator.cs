using ExamiNation.Domain.Entities.Test;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IScoreCalculator
    {
        decimal CalculateScore(ICollection<Answer> answers, ICollection<Question> questions);
    }
}
