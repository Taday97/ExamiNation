using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Answer
{
    public class CreateAnswerDto
    {
        [Required]
        public Guid QuestionId { get; set; }

        public Guid? OptionId { get; set; }
        public string? Text { get; set; }
        public Guid TestResultId { get;  set; }
    }
}
