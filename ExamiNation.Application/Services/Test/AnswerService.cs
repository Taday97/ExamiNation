using AutoMapper;
using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Question;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Repositories.Test;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ApiResponse<IEnumerable<AnswerDto>>> GetAllByTestAsync(Guid answerId)
        {
            if (answerId == Guid.Empty)
            {
                return ApiResponse<IEnumerable<AnswerDto>>.CreateErrorResponse("Invalid answer ID.");
            }
            var options = new QueryOptions<Answer>
            {
                Filter = l => l.TestResult != null && l.TestResult.TestId == answerId,
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

            try
            {
                var answerDelete = await _answerRepository.DeleteAsync(guid);

            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
            {
                return ApiResponse<AnswerDto>.CreateErrorResponse(
                  "This answer cannot be deleted because it is being used in another entity."
               );
            }

            var answerDto = _mapper.Map<AnswerDto>(answer);

            return ApiResponse<AnswerDto>.CreateSuccessResponse("Answer deleted successfully.", answerDto);
        }

        public async Task<ApiResponse<PagedResponse<AnswerDto>>> GetAllPagedAsync(QueryParameters queryParameters)
        {
            var optionsQuery = _mapper.Map<PagedQueryOptions<Answer>>(queryParameters);

            var (answers, totalCount) = await _answerRepository.GetPagedWithCountAsync(optionsQuery);

            var answerDtos = _mapper.Map<IEnumerable<AnswerDto>>(answers);
            var result = _mapper.Map<PagedResponse<AnswerDto>>(queryParameters);

            result.Items = answerDtos;
            result.TotalCount = totalCount;

            return ApiResponse<PagedResponse<AnswerDto>>.CreateSuccessResponse("Answeres retrieved successfully.", result);
        }

        public async Task<ApiResponse<PagedResponse<AnswerResultDetailDto>>> GetResultDetails(QueryParameters queryParameters)
        {
            var optionsQuery = _mapper.Map<PagedQueryOptions<Answer>>(queryParameters);
            optionsQuery.Includes = new List<Expression<Func<Answer, object>>>
            {
                l => l.Question,
                l => l.Option
            };
            optionsQuery.ThenIncludes.Add(q => q.Include(x => x.Question).ThenInclude(a => a.Options));

            var (answers, totalCount) = await _answerRepository.GetPagedWithCountAsync(optionsQuery);


            var answerDtos = answers.Select(l => new AnswerResultDetailDto
            {
                TestResultId = l.TestResultId,
                QuestionNumber = l.Question.QuestionNumber,
                QuestionText = l.Question.Text,
                CorrectAnswerText = l.Question.Options.FirstOrDefault(l => l.IsCorrect)?.Text ?? "",
                UserAnswerText = l.Option.Text,
                IsCorrect = l.Option.IsCorrect,

            }).ToList();

            var result = _mapper.Map<PagedResponse<AnswerResultDetailDto>>(queryParameters);

            result.Items = answerDtos;
            result.TotalCount = totalCount;

            return ApiResponse<PagedResponse<AnswerResultDetailDto>>.CreateSuccessResponse("Answeres retrieved successfully.", result);
        }
    }
}
