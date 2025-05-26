using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.DTOs.CognitiveCategory
{
    public class CognitiveCategoryResultDto
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public int TotalQuestions { get; set; }

        public decimal CorrectAnswers { get; set; }
        public int AnsweredQuestions { get; set; }

        public double PercentageCorrect => TotalQuestions == 0
            ? 0
            : Math.Round((double)CorrectAnswers / TotalQuestions * 100, 2);

        public double ProgressPercentage => TotalQuestions == 0
            ? 0
            : Math.Round((double)AnsweredQuestions / TotalQuestions * 100, 2);
    }
}
