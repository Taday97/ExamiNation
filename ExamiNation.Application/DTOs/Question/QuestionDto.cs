using ExamiNation.Application.DTOs.Option;
using ExamiNation.Domain.Enums;

namespace ExamiNation.Application.DTOs.Test
{
    public class QuestionDto
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public QuestionType Type { get; set; }

        public Guid TestId { get; set; }

        public string TestText { get; set; }

        public List<OptionDto> Options { get; set; } = new();
    }
}
