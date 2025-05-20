using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Entities.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Services.Test
{
    public class StandardScoreCalculator : IScoreCalculator
    {
        public decimal CalculateScore(ICollection<Answer> answers, ICollection<Question> questions)
        {
            int correctCount = 0;
            int validQuestionCount = 0;

            foreach (var question in questions)
            {
                if (question.Options == null || !question.Options.Any())
                    continue;

                validQuestionCount++;

                var correctOption = question.Options.Where(o => o.IsCorrect);
                var userAnswer = answers.FirstOrDefault(a => a.QuestionId == question.Id);

                if (userAnswer != null && correctOption.Any(o => o.Id == userAnswer.OptionId))
                {
                    correctCount++;
                }
            }

            return (decimal)correctCount;
        }
    }
}
