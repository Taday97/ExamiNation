using AutoMapper;
using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Domain.Interfaces.Test;

namespace ExamiNation.Application.Services.Test
{
    public class TestResultService : ITestResultService
    {
        private readonly ITestResultRepository _questionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public TestResultService(ITestResultRepository questionRepository, IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<TestResultDto>>> GetAllAsync()
        {
            var questions = await _questionRepository.GetTestResultsAsync();

            if (questions == null || !questions.Any())
            {
                return ApiResponse<IEnumerable<TestResultDto>>.CreateErrorResponse("No questions found.");
            }

            var questionDtos = _mapper.Map<IEnumerable<TestResultDto>>(questions);

            return ApiResponse<IEnumerable<TestResultDto>>.CreateSuccessResponse("TestResult retrieved successfully.", questionDtos);
        }
        public async Task<ApiResponse<TestResultDto>> GetByIdAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse("TestResult ID must be a valid GUID.");
            }
            var question = await _questionRepository.GetByIdAsync(guid);
            if (question == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse($"TestResult with id {id} not found.");
            }

            var questionDto = _mapper.Map<TestResultDto>(question);
            return ApiResponse<TestResultDto>.CreateSuccessResponse("TestResult retrieved successfully.", questionDto);
        }

        public async Task<ApiResponse<IEnumerable<TestResultDto>>> GetByTestIdAsync(Guid testId)
        {
            var questions = await _questionRepository.GetTestResultsAsync(l => l.TestId == testId);

            if (questions == null || !questions.Any())
            {
                return ApiResponse<IEnumerable<TestResultDto>>.CreateErrorResponse("No questions found.");
            }

            var questionDtos = _mapper.Map<IEnumerable<TestResultDto>>(questions);

            return ApiResponse<IEnumerable<TestResultDto>>.CreateSuccessResponse("TestResult retrieved successfully.", questionDtos);
        }

        public async Task<ApiResponse<TestResultDto>> AddAsync(CreateTestResultDto questionDto)
        {
            if (questionDto == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse("TestResult data cannot be null.");
            }

            var questionEntity = _mapper.Map<TestResult>(questionDto);

            var createdTestResult = await _questionRepository.AddAsync(questionEntity);

            var createdTestResultDto = _mapper.Map<TestResultDto>(createdTestResult);

            return ApiResponse<TestResultDto>.CreateSuccessResponse("TestResult created successfully.", createdTestResultDto);

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

            var question = await _questionRepository.GetByIdAsync(guid);
            if (question == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse($"TestResult with id {id} not found.");
            }

            var rolDelete = await _questionRepository.DeleteAsync(guid);

            var questionDto = _mapper.Map<TestResultDto>(question);

            return ApiResponse<TestResultDto>.CreateSuccessResponse("TestResult deleted successfully.", questionDto);
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
            var question = await _questionRepository.GetByIdAsync(guid);
            if (question == null)
            {
                return ApiResponse<TestResultDto>.CreateErrorResponse($"TestResult with id {editTestResultDto.Id} not found.");
            }

            _mapper.Map(editTestResultDto, question);

            SyncAnswers(question, editTestResultDto.Answers);

            await _questionRepository.UpdateAsync(question);

            TestResultDto questionDto = _mapper.Map<TestResultDto>(question);
            return ApiResponse<TestResultDto>.CreateSuccessResponse("TestResult updated successfully.", questionDto);
        }

        private void SyncAnswers(TestResult question, List<EditAnswerDto>? updatedAnswers)
        {
            updatedAnswers ??= new List<EditAnswerDto>();

            var toRemove = question.Answers
                .Where(opt => !updatedAnswers.Any(u => u.Id == opt.Id))
                .ToList();

            foreach (var option in toRemove)
            {
                question.Answers.Remove(option);
            }

            foreach (var updated in updatedAnswers)
            {
                var existingAnswer = question.Answers.FirstOrDefault(o => o.Id == updated.Id);
                if (existingAnswer != null)
                {
                    _mapper.Map(updated, existingAnswer);
                }
                else
                {
                    var newAnswer = _mapper.Map<Answer>(updated);
                    newAnswer.Id = Guid.NewGuid();
                    newAnswer.TestResultId = question.Id;
                    question.Answers.Add(newAnswer);
                }
            }
        }

       
    }
}
