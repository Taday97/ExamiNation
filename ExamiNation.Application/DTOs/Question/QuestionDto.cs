using ExamiNation.Application.DTOs.Option;
using ExamiNation.Domain.Enums;

namespace ExamiNation.Application.DTOs.Question
{
    public class QuestionDto
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public QuestionType Type { get; set; }

        public Guid TestId { get; set; }
        public decimal Score { get; set; } = 1.0m;

        public int? QuestionNumber { get; set; }

    }
}
