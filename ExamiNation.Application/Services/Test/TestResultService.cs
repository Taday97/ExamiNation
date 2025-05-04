using AutoMapper;
using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Enums;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Repositories.Test;

namespace ExamiNation.Application.Services.Test
{
    public class TestResultService : ITestResultService
    {
        private readonly ITestResultRepository _testResultRepository;
        private readonly ITestRepository _testRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public TestResultService(ITestResultRepository testResultRepository, ITestRepository testRepository, IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _testResultRepository = testResultRepository;
            _testRepository = testRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<TestResultDto>>> GetAllAsync()
        {
            var testResult = await _testResultRepository.GetTestResultsAsync();

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

            var testResult = await _testResultRepository.GetTestResultsAsync(l => l.Status == status && l.UserId == usertId);

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
            var testResult = await _testResultRepository.GetTestResultsAsync(l => l.TestId == testId);

            if (testResult == null || !testResult.Any())
            {
                return ApiResponse<IEnumerable<TestResultDto>>.CreateErrorResponse("No testResult found.");
            }

            var testResultDtos = _mapper.Map<IEnumerable<TestResultDto>>(testResult);

            return ApiResponse<IEnumerable<TestResultDto>>.CreateSuccessResponse("TestResult retrieved successfully.", testResultDtos);
        }

        public async Task<ApiResponse<IEnumerable<TestResultDto>>> GetByUserIdAsync(Guid userId)
        {
            var testResult = await _testResultRepository.GetTestResultsAsync(l => l.UserId == userId);

            if (testResult == null || !testResult.Any())
            {
                return ApiResponse<IEnumerable<TestResultDto>>.CreateErrorResponse("No TestResult found.");
            }

            var testResultDtos = _mapper.Map<IEnumerable<TestResultDto>>(testResult);

            return ApiResponse<IEnumerable<TestResultDto>>.CreateSuccessResponse("TestResult retrieved successfully.", testResultDtos);
        }

        public async Task<ApiResponse<TestResultDto>> AddAsync(CreateTestResultDto testResultDto)
        {
            if (testResultDto == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse("TestResult data cannot be null.");
            }

            var testResultEntity = _mapper.Map<TestResult>(testResultDto);

            
            var test = await _testRepository.GetByIdAsync(testResultDto.TestId);

            if (test == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse("Test not found.");
            }

            var totalQuestions = test.Questions.Count;
            var answeredQuestions = testResultDto.Answers.Count;

            if (answeredQuestions == 0)
            {
                testResultEntity.Status = TestResultStatus.Abandoned;
            }
            else if (answeredQuestions < totalQuestions)
            {
                testResultEntity.Status = TestResultStatus.InProgress;
            }
            else
            {
                testResultEntity.Status = TestResultStatus.Completed;
                testResultEntity.Score = await CalcularScoreAsync(testResultDto.Answers, test.Questions);
            }

            var createdTestResult = await _testResultRepository.AddAsync(testResultEntity);

            var createdTestResultDto = _mapper.Map<TestResultDto>(createdTestResult);

            return ApiResponse<TestResultDto>.CreateSuccessResponse("TestResult created successfully.", createdTestResultDto);
        }

        private async Task<decimal> CalcularScoreAsync(List<CreateAnswerDto> answers, ICollection<Question> questions)
        {
            int correctCount = 0;

            foreach (var question in questions)
            {
                var correctOption = question.Options.Where(o => o.IsCorrect);
                var userAnswer = answers.FirstOrDefault(a => a.QuestionId == question.Id);

                if (userAnswer != null && correctOption.Any(L=>L.Id.Equals(userAnswer.OptionId)))
                {
                    correctCount++;
                }
            }

            return questions.Count == 0 ? 0 : (decimal)correctCount / questions.Count * 100;
        }

        public async Task<ApiResponse<TestResultDto>> Delete(Guid id)
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

        public async Task<ApiResponse<TestResultDto>> Update(EditTestResultDto editTestResultDto)
        {
            if (editTestResultDto == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse("TestResult data cannot be null.");
            }

            if (!Guid.TryParse(editTestResultDto.Id.ToString(), out var guid))
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse("TestResult ID must be a valid GUID.");
            }

            var testResult = await _testResultRepository.GetByIdWithAnswersAsync(guid); 
            if (testResult == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse($"TestResult with id {editTestResultDto.Id} not found.");
            }

            _mapper.Map(editTestResultDto, testResult);

            SyncAnswers(testResult, editTestResultDto.Answers);

            var test = await _testRepository.GetByIdWithQuestionsAsync(testResult.TestId);

            var totalQuestions = test.Questions.Count;
            var answeredQuestions = testResult.Answers.Count;

            if (answeredQuestions == 0)
            {
                testResult.Status = TestResultStatus.Abandoned;
            }
            else if (answeredQuestions < totalQuestions)
            {
                testResult.Status = TestResultStatus.InProgress;
            }
            else
            {
                testResult.Status = TestResultStatus.Completed;
                var createAnswerDto = _mapper.Map <List< CreateAnswerDto>>(testResult.Answers);
                testResult.Score = await CalcularScoreAsync(createAnswerDto, test.Questions.ToList());
            }

            await _testResultRepository.UpdateAsync(testResult);

            var testResultDto = _mapper.Map<TestResultDto>(testResult);
            return ApiResponse<TestResultDto>.CreateSuccessResponse("TestResult updated successfully.", testResultDto);
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

       
    }
}
