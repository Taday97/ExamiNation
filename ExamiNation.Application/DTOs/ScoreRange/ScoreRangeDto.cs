using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.ScoreRange
{
    public class ScoreRangeDto
    {
        public Guid Id { get; set; }

        public Guid TestId { get; set; }
        public string TestName { get; set; }

        public int MinScore { get; set; }

        public int MaxScore { get; set; }

        public string Classification { get; set; }

        [Required]
        public string ShortDescription { get; set; }  // "Your score places you in the Superior intelligence range."

        [Required]
        public string DetailedExplanation { get; set; } // List of strengths, insights, and expanded description

        public string Recommendations { get; set; } // Optional: personal or professional development suggestions
    }
}
