using AutoMapper;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Application.Interfaces;
using ExamiNation.Domain.Common;
using System.Linq.Expressions;

namespace ExamiNation.Application.Services.Test
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AnswerService(IAnswerRepository answerRepository, IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _answerRepository = answerRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<AnswerDto>>> GetAllAsync()
        {
            var answers = await _answerRepository.GetAllAsync();

            if (answers == null || !answers.Any())
            {
                return ApiResponse<IEnumerable<AnswerDto>>.CreateErrorResponse("No answers found.");
            }

            var answerDtos = _mapper.Map<IEnumerable<AnswerDto>>(answers);

            return ApiResponse<IEnumerable<AnswerDto>>.CreateSuccessResponse("Answer retrieved successfully.", answerDtos);
        }
        public async Task<ApiResponse<IEnumerable<AnswerDto>>> GetAllByTestAsync(Guid testId)
        {
            if (testId == Guid.Empty)
            {
                return ApiResponse<IEnumerable<AnswerDto>>.CreateErrorResponse("Invalid test ID.");
            }
            var options = new QueryOptions<Answer>
            {
                Filter = l => l.TestResult != null && l.TestResult.TestId == testId,
                AsNoTracking = true,
                Includes = new List<Expression<Func<Answer, object>>>
                {
                   l => l.TestResult
                }
            };

            var answers = await _answerRepository.GetAllAsync(options);

            if (answers == null || !answers.Any())
            {
                return ApiResponse<IEnumerable<AnswerDto>>.CreateErrorResponse("No answers found.");
            }

            var answerDtos = _mapper.Map<IEnumerable<AnswerDto>>(answers);

            return ApiResponse<IEnumerable<AnswerDto>>.CreateSuccessResponse("Answers retrieved successfully.", answerDtos);
        }

        public async Task<ApiResponse<AnswerDto>> GetByIdAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<AnswerDto>.CreateErrorResponse("Answer ID must be a valid GUID.");
            }
            var answer = await _answerRepository.GetByIdAsync(guid);
            if (answer == null)
            {
                return ApiResponse<AnswerDto>.CreateErrorResponse($"Answer with id {id} not found.");
            }

            var answerDto = _mapper.Map<AnswerDto>(answer);
            return ApiResponse<AnswerDto>.CreateSuccessResponse("Answer retrieved successfully.", answerDto);
        }

        public async Task<ApiResponse<AnswerDto>> AddAsync(CreateAnswerDto answerDto)
        {
            if (answerDto == null)
            {
                return ApiResponse<AnswerDto>.CreateErrorResponse("Answer data cannot be null.");
            }

            var answerEntity = _mapper.Map<Answer>(answerDto);

            var createdAnswer = await _answerRepository.AddAsync(answerEntity);

            var createdAnswerDto = _mapper.Map<AnswerDto>(createdAnswer);

            return ApiResponse<AnswerDto>.CreateSuccessResponse("Answer created successfully.", createdAnswerDto);

        }

        public async Task<ApiResponse<AnswerDto>> UpdateAsync(EditAnswerDto editDto)
        {
            if (editDto == null)
            {
                return ApiResponse<AnswerDto>.CreateErrorResponse("Answer data cannot be null.");
            }
            if (!Guid.TryParse(editDto.Id.ToString(), out var guid))
            {
                return ApiResponse<AnswerDto>.CreateErrorResponse("Answer ID must be a valid GUID.");
            }
            var answer = await _answerRepository.GetByIdAsync(guid);
            if (answer == null)
            {
                return ApiResponse<AnswerDto>.CreateErrorResponse($"Answer with id {editDto.Id} not found.");
            }

            _mapper.Map(editDto, answer);

            await _answerRepository.UpdateAsync(answer);

            AnswerDto answerDto = _mapper.Map<AnswerDto>(answer);
            return ApiResponse<AnswerDto>.CreateSuccessResponse("Answer updated successfully.", answerDto);
        }

        public async Task<ApiResponse<AnswerDto>> DeleteAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<AnswerDto>.CreateErrorResponse("Answer ID must be a valid GUID.");
            }
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return ApiResponse<AnswerDto>.CreateErrorResponse("Answer ID is required.");
            }

            var answer = await _answerRepository.GetByIdAsync(guid);
            if (answer == null)
            {
                return ApiResponse<AnswerDto>.CreateErrorResponse($"Answer with id {id} not found.");
            }

            var rolDelete = await _answerRepository.DeleteAsync(guid);

            var answerDto = _mapper.Map<AnswerDto>(answer);

            return ApiResponse<AnswerDto>.CreateSuccessResponse("Answer deleted successfully.", answerDto);
        }
    }
}
