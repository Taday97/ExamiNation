using ExamiNation.Application.DTOs.Option;
using ExamiNation.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Question
{
    public class EditQuestionDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required, StringLength(500)]
        public string Text { get; set; }

        [Required]
        public QuestionType Type { get; set; }

        [Required]
        public Guid TestId { get; set; }
        public Guid? CognitiveCategoryId { get; set; }
        public decimal Score { get; set; } = 1.0m;

        public int? QuestionNumber { get; set; }

        public List<OptionDto>? Options { get; set; }
    }

}
