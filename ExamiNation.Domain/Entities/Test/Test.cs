using ExamiNation.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Domain.Entities.Test
{
    public class Test
    {
        [Key]
        public Guid Id { get; private set; }

        [Required, StringLength(100)]
        public string Name { get;  set; }

        [StringLength(500)]
        public string? Description { get;  set; }

        [Required]
        public TestType Type { get;  set; }

        [Required]
        public DateTime CreatedAt { get;  set; }

        public string? ImageUrl { get; set; }
        public virtual ICollection<Question> Questions { get;  set; }
        public virtual ICollection<TestResult> TestResults { get;  set; }
        public virtual ICollection<ScoreRange> ScoreRanges { get;  set; }
        
    }

}
