using ExamiNation.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.ScoreRange
{
    public class ScoreRangeDto
    {
        public Guid Id { get; set; }

        public Guid TestId { get; set; }
        public string TestName { get; set; }
        public TestType TestType { get; set; }

        public int MinScore { get; set; }

        public int MaxScore { get; set; }

        public string Classification { get; set; }

        [Required]
        public string ShortDescription { get; set; }  

        [Required]
        public string DetailedExplanation { get; set; } 

        public string Recommendations { get; set; } 
    }
}
