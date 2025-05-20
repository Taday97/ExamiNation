using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.TestResult
{
    public class SubmitAnswerDto
    {
        [Required]
        public Guid QuestionId { get; set; }
        [Required]
        public Guid SelectedOptionId { get; set; }
    }
}
