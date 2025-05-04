using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.DTOs.Role;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Application.DTOs.User;
using ExamiNation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.TestResult
{
    public class TestResultDto
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public UserDto User { get; set; }

        [Required]
        public Guid TestId { get; set; }
        public TestDto Test { get; set; }

        [Required]
        [Precision(10, 4)]
        public decimal Score { get; set; }

        public TestResultStatus Status { get; set; }

        public List<AnswerDto> Answers { get; set; }
    }
}
