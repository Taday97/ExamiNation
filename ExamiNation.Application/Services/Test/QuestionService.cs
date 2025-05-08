using AutoMapper;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Option;
using ExamiNation.Application.DTOs.Question;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Repositories.Test;
using System.Linq.Expressions;

namespace ExamiNation.Application.Services.Test
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public QuestionService(IQuestionRepository questionRepository, IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<QuestionDto>>> GetAllAsync()
        {
            var options = new QueryOptions<Question>
            {
                OrderBy = q => q.OrderBy(p => p.QuestionNumber),
                Includes = new List<Expression<Func<Question, object>>>
                {
                 l => l.Test
                }
            };

            var questions = await _questionRepository.GetAllAsync(options);

            if (questions == null || !questions.Any())
            {
                return ApiResponse<IEnumerable<QuestionDto>>.CreateErrorResponse("No questions found.");
            }

            var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);

            return ApiResponse<IEnumerable<QuestionDto>>.CreateSuccessResponse("Question retrieved successfully.", questionDtos);
        }
        public async Task<ApiResponse<QuestionDto>> GetByIdAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<QuestionDto>.CreateErrorResponse("Question ID must be a valid GUID.");
            }
            var question = await _questionRepository.GetByIdAsync(guid, true, q => q.Options, q => q.Test);
            if (question == null)
            {
                return ApiResponse<QuestionDto>.CreateErrorResponse($"Question with id {id} not found.");
            }

            var questionDto = _mapper.Map<QuestionDtoWithOptions>(question);
            return ApiResponse<QuestionDto>.CreateSuccessResponse("Question retrieved successfully.", questionDto);
        }

        public async Task<ApiResponse<IEnumerable<QuestionDto>>> GetByTestIdAsync(Guid testId)
        {
            if (testId == Guid.Empty)
            {
                return ApiResponse<IEnumerable<QuestionDto>>.CreateErrorResponse("Test ID is invalid.");
            }

            var options = new QueryOptions<Question>
            {
                Filter = q => q.TestId == testId,
                OrderBy = q => q.OrderBy(p => p.QuestionNumber),
            };

            var questions = await _questionRepository.GetAllAsync(options);

            if (questions == null || !questions.Any())
            {
                return ApiResponse<IEnumerable<QuestionDto>>.CreateErrorResponse("No questions found for the specified test.");
            }

            var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);

            return ApiResponse<IEnumerable<QuestionDto>>.CreateSuccessResponse("Questions retrieved successfully.", questionDtos);
        }
        public async Task<ApiResponse<PagedResponse<QuestionDto>>> GetAllPagedAsync(QueryParameters queryParameters)
        {
            var options = new PagedQueryOptions<Question>
            {
                Filters = queryParameters.Filters,
                SortBy = queryParameters.SortBy,
                SortDescending = queryParameters.SortDescending,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
            };


            var (questions, totalCount) = await _questionRepository.GetPagedWithCountAsync(options);

            if (!questions.Any())
            {
                return ApiResponse<PagedResponse<QuestionDto>>.CreateErrorResponse("No questions found.");
            }

            var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);

            var result = new PagedResponse<QuestionDto>
            {
                Items = questionDtos,
                TotalCount = totalCount,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                Filters = queryParameters.Filters,
            };

            return ApiResponse<PagedResponse<QuestionDto>>.CreateSuccessResponse("Tests retrieved successfully.", result);
        }

        public async Task<ApiResponse<QuestionDto>> AddAsync(CreateQuestionDto questionDto)
        {
            if (questionDto == null)
            {
                return ApiResponse<QuestionDto>.CreateErrorResponse("Question data cannot be null.");
            }

            var questionEntity = _mapper.Map<Question>(questionDto);

            var createdQuestion = await _questionRepository.AddAsync(questionEntity);

            var createdQuestionDto = _mapper.Map<QuestionDto>(createdQuestion);

            return ApiResponse<QuestionDto>.CreateSuccessResponse("Question created successfully.", createdQuestionDto);

        }

        public async Task<ApiResponse<QuestionDto>> DeleteAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<QuestionDto>.CreateErrorResponse("Question ID must be a valid GUID.");
            }
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return ApiResponse<QuestionDto>.CreateErrorResponse("Question ID is required.");
            }

            var question = await _questionRepository.GetByIdAsync(guid);
            if (question == null)
            {
                return ApiResponse<QuestionDto>.CreateErrorResponse($"Question with id {id} not found.");
            }

            var rolDelete = await _questionRepository.DeleteAsync(guid);

            var questionDto = _mapper.Map<QuestionDto>(question);

            return ApiResponse<QuestionDto>.CreateSuccessResponse("Question deleted successfully.", questionDto);
        }

        public async Task<ApiResponse<QuestionDto>> UpdateAsync(EditQuestionDto editQuestionDto)
        {
            if (editQuestionDto == null)
            {
                return ApiResponse<QuestionDto>.CreateErrorResponse("Question data cannot be null.");
            }
            if (!Guid.TryParse(editQuestionDto.Id.ToString(), out var guid))
            {
                return ApiResponse<QuestionDto>.CreateErrorResponse("Question ID must be a valid GUID.");
            }
            var question = await _questionRepository.GetByIdAsync(guid);
            if (question == null)
            {
                return ApiResponse<QuestionDto>.CreateErrorResponse($"Question with id {editQuestionDto.Id} not found.");
            }

            _mapper.Map(editQuestionDto, question);

            SyncOptions(question, editQuestionDto.Options);

            await _questionRepository.UpdateAsync(question);

            QuestionDto questionDto = _mapper.Map<QuestionDto>(question);
            return ApiResponse<QuestionDto>.CreateSuccessResponse("Question updated successfully.", questionDto);
        }

        private void SyncOptions(Question question, List<EditOptionDto>? updatedOptions)
        {
            updatedOptions ??= new List<EditOptionDto>();

            var toRemove = question.Options
                .Where(opt => !updatedOptions.Any(u => u.Id == opt.Id))
                .ToList();

            foreach (var option in toRemove)
            {
                question.Options.Remove(option);
            }

            foreach (var updated in updatedOptions)
            {
                var existingOption = question.Options.FirstOrDefault(o => o.Id == updated.Id);
                if (existingOption != null)
                {
                    _mapper.Map(updated, existingOption);
                }
                else
                {
                    var newOption = _mapper.Map<Option>(updated);
                    newOption.Id = Guid.NewGuid();
                    newOption.QuestionId = question.Id;
                    question.Options.Add(newOption);
                }
            }
        }

       
    }
}
