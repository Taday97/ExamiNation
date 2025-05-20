using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.DTOs.Option;
using ExamiNation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.TestResult
{
    public class EditTestResultDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid TestId { get; set; }

        public decimal Score { get; set; }

        public TestResultStatus Status { get; set; }

        public List<EditAnswerDto>? Answers { get; set; }
        public DateTime? CompletedAt { get; internal set; }
        public DateTime? StartedAt { get; internal set; }
    }

}
