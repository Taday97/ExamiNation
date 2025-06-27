using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.DTOs.Answer
{
    public class AnswerResultDetailDto
    {
        public Guid TestResultId { get; set; }
        public int? QuestionNumber { get; set; }
        public string QuestionText { get; set; }
        public string UserAnswerText { get; set; }
        public string CorrectAnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
