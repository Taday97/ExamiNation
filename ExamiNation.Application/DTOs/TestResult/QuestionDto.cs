using ExamiNation.Application.DTOs.Option;
using ExamiNation.Application.DTOs.Role;
using ExamiNation.Domain.Entities.Security;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Test
{
    public class TestResultDto
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public Guid TestId { get; set; }
        public string TestText { get; set; }

        [Required]
        [Precision(10, 4)]
        public decimal Score { get; set; }

        public TestResultStatus Status { get; set; }

        public List<AnswerDto> Answers { get; set; }
    }
}
