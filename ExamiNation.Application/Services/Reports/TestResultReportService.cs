using AutoMapper;
using ExamiNation.Application.DTOs.CognitiveCategory;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Application.Interfaces.Reports;
using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Services.Reports
{
    public class TestResultReportService : ITestResultReportService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IMapper _mapper;

        public TestResultReportService(IQuestionRepository questionRepository, IAnswerRepository answerRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _mapper = mapper;
        }

        public async Task<List<TestResultReportDto>> TestResultsReportsMapping(IEnumerable<TestResult> testResults)
        {
            var dtos = new List<TestResultReportDto>();

            foreach (var result in testResults)
            {
                var answers = await _answerRepository.GetAllAsync(new QueryOptions<Answer>
                {
                    Filter = a => a.TestResultId == result.Id
                });

                var answeredQuestionIds = answers.Select(a => a.QuestionId).ToHashSet();

                var questions = await _questionRepository.GetAllAsync(new QueryOptions<Question>
                {
                    Filter = q => q.TestId == result.TestId,
                    OrderBy = q => q.OrderBy(q => q.QuestionNumber)
                });

                var nextQuestion = questions
                    .FirstOrDefault(q => !answeredQuestionIds.Contains(q.Id));

                var dto = _mapper.Map<TestResultReportDto>(result);
                dto.QuestionCount = questions.Count();
                dto.AnsweredCount = answeredQuestionIds.Count;
                dto.CategoryResults = await CalculateCategoryResults(result);

                if (nextQuestion != null)
                {
                    dto.NextQuestionPage = questions.ToList().FindIndex(q => q.Id == nextQuestion.Id) + 1;
                }

                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<List<CognitiveCategoryResultDto>> CalculateCategoryResults(TestResult result)
        {
            var questions = await _questionRepository.GetAllAsync(new QueryOptions<Question>
            {
                Filter = q => q.TestId == result.TestId && q.CognitiveCategoryId != null,
                Includes = new List<Expression<Func<Question, object>>>
        {
            q => q.CognitiveCategory,
            q => q.Options
        }
            });

            var answers = await _answerRepository.GetAllAsync(new QueryOptions<Answer>
            {
                Filter = a => a.TestResultId == result.Id
            });
            var answeredQuestionIds = answers.Select(a => a.QuestionId).ToHashSet();

            var categoryResults = questions
                .Where(q => q.CognitiveCategory != null)
                .GroupBy(q => q.CognitiveCategory.Code)
                .Select(group =>
                {
                    var categoryQuestions = group.ToList();
                    var category = categoryQuestions.First().CognitiveCategory;

                    var totalQuestions = categoryQuestions.Count;
                    var answeredCount = categoryQuestions.Count(q => answeredQuestionIds.Contains(q.Id));
                    var correctScore = categoryQuestions.Sum(q =>
                        answers.Any(a =>
                            a.QuestionId == q.Id &&
                            q.Options.Any(o => o.Id == a.OptionId && o.IsCorrect))
                            ? q.Score
                            : 0
                    );

                    return new CognitiveCategoryResultDto
                    {
                        Code = category.Code,
                        Name = category.Name,
                        TotalQuestions = totalQuestions,
                        AnsweredQuestions = answeredCount,
                        CorrectAnswers = correctScore
                    };
                })
                .ToList();

            return categoryResults;
        }

    }
}
