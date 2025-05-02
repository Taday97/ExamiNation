using ExamiNation.Application.DTOs.Option;
using ExamiNation.Application.DTOs.Role;
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

        [Required]
        [Precision(10, 4)]
        public decimal Score { get; set; }

        public TestResultStatus Status { get; set; }

        public List<CreateAnswerDto>? Answers { get; set; }
    }
}
