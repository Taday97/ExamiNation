using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Answer
{
    public class AnswerDto
    {

        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TestResultId { get; set; }

        [Required]
        public Guid QuestionId { get; set; }
        public string? QuestionText { get; set; }

        public Guid? OptionId { get; set; }
        public string? OptionText { get; set; }

        [StringLength(1000)]
        public string? Text { get; set; }
    }
}
