using ExamiNation.Application.DTOs.CognitiveCategory;
using ExamiNation.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.DTOs.TestResult
{
    public class TestResultReportDto : TestResultDto
    {
        public int QuestionCount { get; set; }
        public int AnsweredCount { get; set; }
        public int NextQuestionPage { get; set; }

        public List<CognitiveCategoryResultDto> CategoryResults { get; set; }

        public double? ProgressPercentage => Math.Round((double)AnsweredCount / QuestionCount * 100, 2);
    }
}
