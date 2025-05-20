using ExamiNation.Application.DTOs.TestResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.DTOs.ScoreRange
{
    public class ScoreRangeDetailsDto : ScoreRangeDto
    {
        public TestResultDto TestResultDto { get; set; }
        public int CountQuestions { get; set; }
        public int CountAnswers { get; set; }
    }
}
