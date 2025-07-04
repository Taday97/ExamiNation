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
using ExamiNation.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace ExamiNation.Application.Services.Test
{
    public class QuestionService : IQuestionService
    {
        private readonly IOptionRepository _optionRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        protected readonly AppDbContext _context;

        public QuestionService(IOptionRepository optionRepository, IQuestionRepository questionRepository, IUserRepository userRepository, IUserService userService, IMapper mapper, AppDbContext context)
        {
            _optionRepository = optionRepository;
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
            _context = context;
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
            var question = await _questionRepository.GetByIdAsync(guid, true, include: q => q.Include(t => t.Options).Include(q => q.Test));
            if (question == null)
            {
                return ApiResponse<QuestionDto>.CreateErrorResponse($"Question with id {id} not found.");
            }

            var questionDto = _mapper.Map<QuestionDetailsDto>(question);
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


            var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);

            return ApiResponse<IEnumerable<QuestionDto>>.CreateSuccessResponse("Questions retrieved successfully.", questionDtos);
        }
        public async Task<ApiResponse<PagedResponse<QuestionDto>>> GetAllPagedAsync(QueryParameters queryParameters)
        {
            var optionsQuery = _mapper.Map<PagedQueryOptions<Question>>(queryParameters);

            var (questions, totalCount) = await _questionRepository.GetPagedWithCountAsync(optionsQuery);

            var questionDtos = _mapper.Map<IEnumerable<QuestionDtoWithOptions>>(questions);

            var result = _mapper.Map<PagedResponse<QuestionDto>>(queryParameters);
            result.Items = questionDtos;
            result.TotalCount = totalCount;

            return ApiResponse<PagedResponse<QuestionDto>>.CreateSuccessResponse("Tests retrieved successfully.", result);

        }
        public async Task<ApiResponse<PagedResponse<QuestionViewDto>>> GetAllQuestionPagedAsync(QueryParameters queryParameters)
        {
            var optionsQuery = _mapper.Map<PagedQueryOptions<Question>>(queryParameters);

            optionsQuery.Includes = new List<Expression<Func<Question, object>>>
            {
               l => l.CognitiveCategory,
               l => l.Test,
            };

            var (questions, totalCount) = await _questionRepository.GetPagedWithCountAsync(optionsQuery);


            var questionDtos = _mapper.Map<IEnumerable<QuestionViewDto>>(questions);

            var result = _mapper.Map<PagedResponse<QuestionViewDto>>(queryParameters);
            result.Items = questionDtos;
            result.TotalCount = totalCount;

            return ApiResponse<PagedResponse<QuestionViewDto>>.CreateSuccessResponse("Questions retrieved successfully.", result);

        }

        public async Task<ApiResponse<QuestionsPagedWithTestDto>> GetAllQuestionWithOptionsTestPagedAsync(QueryParameters queryParameters)
        {
            var optionsQuery = _mapper.Map<PagedQueryOptions<Question>>(queryParameters);

            optionsQuery.Includes = new List<Expression<Func<Question, object>>>
            {
               l => l.Options,
               l => l.Test,
               l => l.Answers,
            };
            optionsQuery.ThenIncludes.Add(q => q.Include(x => x.Answers).ThenInclude(a => a.TestResult));

            var (questions, totalCount) = await _questionRepository.GetPagedWithCountAsync(optionsQuery);

            var questionDtos = _mapper.Map<IEnumerable<QuestionDtoWithOptions>>(questions);

            var testDto = _mapper.Map<TestDto>(questions.FirstOrDefault()?.Test);

            var result = _mapper.Map<PagedResponse<QuestionDtoWithOptions>>(queryParameters);
            result.Items = questionDtos;
            result.TotalCount = totalCount;

            var response = new QuestionsPagedWithTestDto
            {
                Test = testDto,
                Questions = result
            };

            return ApiResponse<QuestionsPagedWithTestDto>.CreateSuccessResponse("Tests retrieved successfully.", response);
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
            try
            {
                var questionDelete = await _questionRepository.DeleteAsync(guid);

            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
            {
                return ApiResponse<QuestionDto>.CreateErrorResponse(
                  "This question cannot be deleted because it has already been used in answers."
               );
            }
            catch
            {
                throw;
            }


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

            try
            {
                await SyncOptions(question, editQuestionDto.Options);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<QuestionDto>.CreateErrorResponse(
                    ex.Message
                );
            }

            await _questionRepository.UpdateAsync(question);

            QuestionDto questionDto = _mapper.Map<QuestionDto>(question);
            return ApiResponse<QuestionDto>.CreateSuccessResponse("Question updated successfully.", questionDto);
        }

        private async Task SyncOptions(Question question, List<OptionDto>? updatedOptions)
        {
            updatedOptions ??= new List<OptionDto>();

            var updatedWithId = updatedOptions.Where(o => o.Id != null).ToList();
            var optionsQuestion = await _optionRepository.GetAllAsync(new QueryOptions<Option>
            {
                Filter = q => q.QuestionId == question.Id,
            });
            var toRemove = optionsQuestion
                .Where(opt => !updatedWithId.Any(u => u.Id == opt.Id))
                .ToList();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var option in toRemove)
                {
                    question.Options.Remove(option);
                    await _optionRepository.DeleteAsync(option.Id);
                }

                foreach (var updated in updatedOptions)
                {
                    var existingOption = question.Options.FirstOrDefault(o => o.Id == updated.Id);
                    if (existingOption != null)
                    {
                        _mapper.Map(updated, existingOption);
                        await _optionRepository.UpdateAsync(existingOption);
                    }
                    else
                    {
                        var newOption = _mapper.Map<Option>(updated);
                        newOption.Question = null;
                        newOption.QuestionId = question.Id;

                        await _optionRepository.AddAsync(newOption);
                        question.Options.Add(newOption);
                    }
                }

                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547) // FK violation
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Options cannot be change because they are being used in answers.", ex);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }




    }
}
