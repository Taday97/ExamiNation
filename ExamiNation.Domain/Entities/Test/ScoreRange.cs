using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Domain.Entities.Test
{
    public class ScoreRange
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TestId { get; set; }
        public Test Test { get; set; }

        [Required]
        public int MinScore { get; set; }

        [Required]
        public int MaxScore { get; set; }

        [Required, StringLength(100)]
        public string Classification { get; set; }
    }

}
