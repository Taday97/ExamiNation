using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.DTOs.ScoreRange
{
    public class EditScoreRangeDto
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TestId { get; set; }

        [Required]
        public int MinScore { get; set; }

        [Required]
        public int MaxScore { get; set; }

        [Required, StringLength(100)]
        public string Classification { get; set; }

        [Required]
        public string ShortDescription { get; set; }  // "Your score places you in the Superior intelligence range."

        [Required]
        public string DetailedExplanation { get; set; } // List of strengths, insights, and expanded description

        public string Recommendations { get; set; } // Optional: personal or professional development suggestions
    }
}
