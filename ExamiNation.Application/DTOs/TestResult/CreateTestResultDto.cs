using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.DTOs.Option;
using ExamiNation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.TestResult
{
    public class CreateTestResultDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid TestId { get; set; }

        public List<CreateAnswerDto>? Answers { get; set; }

        public DateTime? StartedAt { get;  set; }
        public DateTime? CompletedAt { get;  set; }
    }
}
