using ExamiNation.Domain.Entities.Security;
using ExamiNation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Domain.Entities.Test
{
    public class TestResult
    {
        [Key]
        public Guid Id { get;  set; }

        [Required]
        public Guid UserId { get;  set; }
        public virtual ApplicationUser User { get;  set; }

        [Required]
        public Guid TestId { get;  set; }
        public Test Test { get;  set; }

        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        [Required]
        [Precision(10, 4)]
        public decimal Score { get;  set; }


        [Required]
        public TestResultStatus Status { get; set; }

        public virtual ICollection<Answer> Answers { get;  set; }
    }

}
