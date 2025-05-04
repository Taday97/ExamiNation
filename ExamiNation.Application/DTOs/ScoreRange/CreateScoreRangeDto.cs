using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.DTOs.ScoreRange
{
    public class CreateScoreRangeDto
    {

        [Required]
        public Guid TestId { get; set; }

        [Required]
        public int MinScore { get; set; }

        [Required]
        public int MaxScore { get; set; }

        [Required, StringLength(100)]
        public string Classification { get; set; }
    }
}
