using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Answer
{
    public class CreateAnswerDto
    {
        [Required]
        public Guid TestResultId { get; set; }

        [Required]
        public Guid QuestionId { get; set; }

        public Guid? OptionId { get; set; }

        [StringLength(1000)]
        public string Text { get; set; }
    }
}
