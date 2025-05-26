using AutoMapper;
using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.CognitiveCategory;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.DTOs.ScoreRange;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Enums;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExamiNation.Application.Services.Test
{
    public class TestResultService : ITestResultService
    {
        private readonly ITestResultRepository _testResultRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ITestRepository _testRepository;
        private readonly IAnswerService _answerService;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IScoreCalculator _scoreCalculator;
        private readonly IScoreRangeService _scoreRangeService;
        private readonly AppDbContext _context;

        public TestResultService(ITestResultRepository testResultRepository, IQuestionRepository questionRepository, ITestRepository testRepository, IAnswerService answerService, IUserRepository userRepository, IUserService userService, IMapper mapper, IScoreCalculator scoreCalculator, IScoreRangeService scoreRangeService, AppDbContext context)
        {
            _testResultRepository = testResultRepository;
            _questionRepository = questionRepository;
            _testRepository = testRepository;
            _answerService = answerService;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
            _scoreCalculator = scoreCalculator;
            _scoreRangeService = scoreRangeService;
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<TestResultDto>>> GetAllAsync()
        {
            var testResult = await _testResultRepository.GetAllAsync();

            if (testResult == null || !testResult.Any())
            {
                return ApiResponse<IEnumerable<TestResultDto>>.CreateErrorResponse("No testResult found.");
            }

            var testResultDtos = _mapper.Map<IEnumerable<TestResultDto>>(testResult);

            return ApiResponse<IEnumerable<TestResultDto>>.CreateSuccessResponse("TestResult retrieved successfully.", testResultDtos);
        }
        public async Task<ApiResponse<IEnumerable<TestResultDto>>> GetAllByStatusUserIdAsync(TestResultStatus status, Guid usertId)
        {
            if (usertId == Guid.Empty)
            {
                return ApiResponse<IEnumerable<TestResultDto>>.CreateErrorResponse("Invalid user ID.");
            }

            if (!Enum.IsDefined(typeof(TestResultStatus), status))
            {
                return ApiResponse<IEnumerable<TestResultDto>>.CreateErrorResponse("Invalid test result status.");
            }
            var options = new QueryOptions<TestResult>
            {
                Filter = l => l.Status == status && l.UserId == usertId
            };

            var testResult = await _testResultRepository.GetAllAsync(options);

            if (testResult == null || !testResult.Any())
            {
                return ApiResponse<IEnumerable<TestResultDto>>.CreateErrorResponse("No test results found.");
            }

            var testResultDtos = _mapper.Map<IEnumerable<TestResultDto>>(testResult);

            return ApiResponse<IEnumerable<TestResultDto>>.CreateSuccessResponse("Test results retrieved successfully.", testResultDtos);
        }

        public async Task<ApiResponse<TestResultDto>> GetByIdAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse("TestResult ID must be a valid GUID.");
            }
            var testResult = await _testResultRepository.GetByIdAsync(guid);
            if (testResult == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse($"TestResult with id {id} not found.");
            }

            var testResultDto = _mapper.Map<TestResultDto>(testResult);
            return ApiResponse<TestResultDto>.CreateSuccessResponse("TestResult retrieved successfully.", testResultDto);
        }

        public async Task<ApiResponse<IEnumerable<TestResultDto>>> GetByTestIdAsync(Guid testId)
        {
            var testResult = await _testResultRepository.GetAllAsync(new QueryOptions<TestResult> { Filter = l => l.TestId == testId });

            if (testResult == null || !testResult.Any())
            {
                return ApiResponse<IEnumerable<TestResultDto>>.CreateErrorResponse("No testResult found.");
            }

            var testResultDtos = _mapper.Map<IEnumerable<TestResultDto>>(testResult);

            return ApiResponse<IEnumerable<TestResultDto>>.CreateSuccessResponse("TestResult retrieved successfully.", testResultDtos);
        }

        public async Task<ApiResponse<IEnumerable<TestResultReportDto>>> GetByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return ApiResponse<IEnumerable<TestResultReportDto>>.CreateErrorResponse("Invalid user ID.");
            }
            var query = new QueryOptions<TestResult>
            {
                Filter = l => l.UserId == userId,
                Includes = new List<Expression<Func<TestResult, object>>>
                {
                     t => t.Test,
                     t => t.Answers,
                },
                ThenIncludes = new List<Func<IQueryable<TestResult>, IQueryable<TestResult>>>
                {
                  q=> q.Include(x => x.Test).ThenInclude(a => a.Questions).ThenInclude(l=>l.Answers).ThenInclude(l=>l.Option),
                  q=> q.Include(x => x.Test).ThenInclude(a => a.Questions).ThenInclude(l=>l.Options),
                }
            };

            var testResult = await _testResultRepository.GetAllAsync(query);

            if (testResult == null || !testResult.Any())
            {
                return ApiResponse<IEnumerable<TestResultReportDto>>.CreateErrorResponse("No TestResult found.");
            }

            List<TestResultReportDto> dtos =await TestResultsReportsMapping(testResult);


            return ApiResponse<IEnumerable<TestResultReportDto>>.CreateSuccessResponse("TestResult retrieved successfully.", dtos);
        }

        private async Task<List<TestResultReportDto>> TestResultsReportsMapping(IEnumerable<TestResult> testResult)
        {
            List<TestResultReportDto> dtos = new List<TestResultReportDto>();
            foreach (var result in testResult)
            {
                var answeredQuestionIds = result.Answers.Select(a => a.QuestionId).ToHashSet();
                var orderedQuestions = result.Test.Questions.OrderBy(q => q.QuestionNumber).ToList();

                var nextQuestion = result.Test.Questions
                    .OrderBy(q => q.QuestionNumber)
                    .FirstOrDefault(q => !answeredQuestionIds.Contains(q.Id));

                var dto = _mapper.Map<TestResultReportDto>(result);
                dto.QuestionCount = result.Test.Questions.Count;
                dto.AnsweredCount = answeredQuestionIds.Count;
                var categoryResults = await CalculateCategoryResults(result);

                dto.CategoryResults = categoryResults;
                if (nextQuestion != null)
                {
                    dto.NextQuestionPage = orderedQuestions.FindIndex(q => q.Id == nextQuestion.Id) + 1;
                }

                dtos.Add(dto);
            }
            return dtos;
        }
        private async Task<List<CognitiveCategoryResultDto>> CalculateCategoryResults(TestResult result)
        {
            var query = new QueryOptions<Question>
            {
                Filter = q => q.Test.Id == result.TestId && q.CognitiveCategoryId!=null,
                Includes = new List<Expression<Func<Question, object>>>
                {
                   q => q.CognitiveCategory,
                   q => q.Options
                }
            };

            var questions = await _questionRepository.GetAllAsync(query);

            var answeredQuestionIds = result.Answers.Select(a => a.QuestionId).ToHashSet();

            var categoryResults = questions
                .Where(q => q.CognitiveCategory != null)
                .GroupBy(q => q.CognitiveCategory.Code)
                .Select(g =>
                {
                    var questionsInCategory = g.ToList();

                    var totalQuestions = questionsInCategory.Count;
                    var answeredInCategory = questionsInCategory.Count(q => answeredQuestionIds.Contains(q.Id));
                    var correctInCategory = questionsInCategory.Sum( q =>
                        result.Answers.Any(a =>
                            a.QuestionId == q.Id &&
                            q.Options.Any(o => o.Id == a.OptionId && o.IsCorrect))
                        ? q.Score : 0);
                    var firstCategory = questionsInCategory.First().CognitiveCategory;
                    return new CognitiveCategoryResultDto
                    {
                        Code = firstCategory.Code,
                        Name = firstCategory.Name,
                        TotalQuestions = totalQuestions,
                        AnsweredQuestions = answeredInCategory,
                        CorrectAnswers = correctInCategory
                    };
                })
                .ToList();

            return categoryResults;
        }


        public async Task<ApiResponse<TestResultDto>> AddAsync(CreateTestResultDto testResultDto)
        {
            if (testResultDto == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse("TestResult data cannot be null.");
            }

            var testResultEntity = _mapper.Map<TestResult>(testResultDto);


            var test = await _testRepository.GetByIdAsync(testResultDto.TestId, true,
               include:
               t => t.Include(t => t.Questions)
              .ThenInclude(question => question.Options));

            if (test == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse("Test not found.");
            }

            var totalQuestions = test.Questions.Count;
            var answeredQuestions = testResultDto.Answers?.Count;


            testResultEntity.Status = answeredQuestions switch
            {
                0 => TestResultStatus.Abandoned,
                _ when answeredQuestions < totalQuestions => TestResultStatus.InProgress,
                _ when answeredQuestions == totalQuestions => TestResultStatus.Completed,
            };

            testResultEntity.Score = _scoreCalculator.CalculateScore(testResultEntity.Answers, test.Questions);
            testResultEntity.CompletedAt = DateTime.UtcNow;
            var createdTestResult = await _testResultRepository.AddAsync(testResultEntity);

            var createdTestResultDto = _mapper.Map<TestResultDto>(createdTestResult);

            return ApiResponse<TestResultDto>.CreateSuccessResponse("TestResult created successfully.", createdTestResultDto);
        }

        public async Task<ApiResponse<TestResultDto>> AddSubmitAnswerAsync(SubmitAnswerDto submitAnswerDto, Guid userId)
        {
            if (submitAnswerDto == null)
                return ApiResponse<TestResultDto>.CreateErrorResponse("SubmitAnswer data cannot be null.");

            var question = await _questionRepository.GetByIdAsync(submitAnswerDto.QuestionId);
            if (question == null)
                return ApiResponse<TestResultDto>.CreateErrorResponse("Question not found.");


            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var testResult = await _testResultRepository
                    .FindFirstAsync(
                        x => x.TestId == question.TestId && x.UserId == userId && x.Status != TestResultStatus.Completed,
                        asNoTracking: false,
                        include: q => q.Include(t => t.Answers)
                       .Include(t => t.Test)
                       .ThenInclude(test => test.Questions)
                       .ThenInclude(question => question.Options)
                    );

                if (testResult == null)
                {
                    var testResultAdd = new CreateTestResultDto();
                    testResultAdd.TestId = question.TestId;
                    testResultAdd.UserId = userId;
                    testResultAdd.StartedAt = DateTime.UtcNow;
                    testResultAdd.Answers = new List<CreateAnswerDto>();

                    var result = await AddAsync(testResultAdd);
                    if (!result.Success)
                        throw new Exception("Failed to create TestResult.");

                    testResult = await _testResultRepository
                        .FindFirstAsync(x => x.Id == result.Data.Id, asNoTracking: false, include: q => q.Include(t => t.Answers)
                       .Include(t => t.Test)
                       .ThenInclude(test => test.Questions)
                       .ThenInclude(question => question.Options));
                }
                else
                {
                    var answerExisting = testResult.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                    if (answerExisting != null)
                    {
                        var answerEdit = new EditAnswerDto
                        {
                            Id = answerExisting.Id,
                            QuestionId = submitAnswerDto.QuestionId,
                            OptionId = submitAnswerDto.SelectedOptionId,
                            TestResultId = answerExisting.TestResultId,
                        };
                        var answerResult = await _answerService.UpdateAsync(answerEdit);
                        if (!answerResult.Success)
                            throw new Exception("Failed to update answer.");
                    }
                    else
                    {
                        var answer = new CreateAnswerDto
                        {
                            QuestionId = question.Id,
                            OptionId = submitAnswerDto.SelectedOptionId,
                            TestResultId = testResult.Id
                        };

                        var answerResult = await _answerService.AddAsync(answer);
                        if (!answerResult.Success)
                            throw new Exception("Failed to save answer.");
                    }


                    await UpdateTestStatusAndScoreAsync(testResult);


                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var testResultDto = _mapper.Map<TestResultDto>(testResult);
                return ApiResponse<TestResultDto>.CreateSuccessResponse("Answer submitted successfully.", testResultDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<TestResultDto>.CreateErrorResponse($"Error submitting answer: {ex.Message}");
            }
        }



        public async Task<ApiResponse<TestResultDto>> DeleteAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse("TestResult ID must be a valid GUID.");
            }
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse("TestResult ID is required.");
            }

            var testResult = await _testResultRepository.GetByIdAsync(guid);
            if (testResult == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse($"TestResult with id {id} not found.");
            }

            var rolDelete = await _testResultRepository.DeleteAsync(guid);

            var testResultDto = _mapper.Map<TestResultDto>(testResult);

            return ApiResponse<TestResultDto>.CreateSuccessResponse("TestResult deleted successfully.", testResultDto);
        }

        public async Task<ApiResponse<TestResultDto>> UpdateAsync(EditTestResultDto editTestResultDto)
        {
            if (editTestResultDto == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse("TestResult data cannot be null.");
            }

            if (!Guid.TryParse(editTestResultDto.Id.ToString(), out var guid))
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse("TestResult ID must be a valid GUID.");
            }

            var testResult = await _testResultRepository.FindFirstAsync(l => l.Id == guid);
            if (testResult == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse($"TestResult with id {editTestResultDto.Id} not found.");
            }

            _mapper.Map(editTestResultDto, testResult);
            SyncAnswers(testResult, editTestResultDto.Answers);

            await UpdateTestStatusAndScoreAsync(testResult);

            var testResultDto = _mapper.Map<TestResultDto>(testResult);
            return ApiResponse<TestResultDto>.CreateSuccessResponse("TestResult updated successfully.", testResultDto);
        }

        public async Task UpdateTestStatusAndScoreAsync(TestResult testResult)
        {
            var test = await _testRepository.FindFirstAsync(
                t => t.Id == testResult.TestId,
                asNoTracking: true,
                q => q.Include(t => t.Questions)
               .ThenInclude(q => q.Options)
            );

            if (test == null)
                throw new InvalidOperationException($"Test with ID {testResult.TestId} was not found.");

            int totalQuestions = test.Questions.Count;
            int answeredQuestions = testResult.Answers.Count;

            testResult.Status = answeredQuestions switch
            {
                0 => TestResultStatus.Abandoned,
                _ when answeredQuestions < totalQuestions => TestResultStatus.InProgress,
                _ when answeredQuestions == totalQuestions => TestResultStatus.Completed,
            };

            testResult.Score = _scoreCalculator.CalculateScore(testResult.Answers, test.Questions);
            testResult.CompletedAt = DateTime.UtcNow;

            await _testResultRepository.UpdateAsync(testResult);
        }


        private void SyncAnswers(TestResult testResult, List<EditAnswerDto>? updatedAnswers)
        {
            updatedAnswers ??= new List<EditAnswerDto>();

            var toRemove = testResult.Answers
                .Where(opt => !updatedAnswers.Any(u => u.Id == opt.Id))
                .ToList();

            foreach (var option in toRemove)
            {
                testResult.Answers.Remove(option);
            }

            foreach (var updated in updatedAnswers)
            {
                var existingAnswer = testResult.Answers.FirstOrDefault(o => o.Id == updated.Id);
                if (existingAnswer != null)
                {
                    _mapper.Map(updated, existingAnswer);
                }
                else
                {
                    var newAnswer = _mapper.Map<Answer>(updated);
                    newAnswer.Id = Guid.NewGuid();
                    newAnswer.TestResultId = testResult.Id;
                    testResult.Answers.Add(newAnswer);
                }
            }
        }

        public async Task<ApiResponse<PagedResponse<TestResultDto>>> GetAllPagedAsync(QueryParameters queryParameters)
        {
            var optionsQuery = _mapper.Map<PagedQueryOptions<TestResult>>(queryParameters);

            var (testResults, totalCount) = await _testResultRepository.GetPagedWithCountAsync(optionsQuery);

            if (!testResults.Any())
            {
                return ApiResponse<PagedResponse<TestResultDto>>.CreateErrorResponse("No testResults found.");
            }

            var testResultDtos = _mapper.Map<IEnumerable<TestResultDto>>(testResults);

            var result = _mapper.Map<PagedResponse<TestResultDto>>(queryParameters);
            result.Items = testResultDtos;
            result.TotalCount = totalCount;

            return ApiResponse<PagedResponse<TestResultDto>>.CreateSuccessResponse("TestResults retrieved successfully.", result);
        }

        public async Task<ApiResponse<ScoreRangeDetailsDto>> GetSummaryAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<ScoreRangeDetailsDto>.CreateErrorResponse("TestResult ID must be a valid GUID.");
            }
            var testResult = await _testResultRepository.GetByIdAsync(guid, asNoTracking: true, include: l => l.Include(m => m.Answers).Include(m => m.Test).ThenInclude(n => n.Questions));

            if (testResult == null)
            {
                return ApiResponse<ScoreRangeDetailsDto>.CreateErrorResponse($"TestResult with id {id} not found.");
            }

            var scoreRange = await _scoreRangeService.GetClasificationAsync(testResult.TestId, testResult.Score);

            if (!scoreRange.Success)
            {
                return ApiResponse<ScoreRangeDetailsDto>.CreateErrorResponse($"ScoreRange with id {testResult.TestId.ToString()} not found.");
            }
            var testScoreRangeDetailsDto = _mapper.Map<ScoreRangeDetailsDto>(scoreRange.Data);
            testScoreRangeDetailsDto.TestResultDto = _mapper.Map<TestResultDto>(testResult);
            testScoreRangeDetailsDto.CountQuestions = testResult.Test.Questions.Count();
            testScoreRangeDetailsDto.CountAnswers = testResult.Answers.Count();


            return ApiResponse<ScoreRangeDetailsDto>.CreateSuccessResponse("Summary retrieved successfully.", testScoreRangeDetailsDto);
        }
    }
}
