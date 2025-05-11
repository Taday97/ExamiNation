using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Question;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IQuestionService : IGenericService<QuestionDto, CreateQuestionDto, EditQuestionDto>
    {
        Task<ApiResponse<IEnumerable<QuestionDto>>> GetByTestIdAsync(Guid testId);
        Task<ApiResponse<PagedResponse<QuestionDtoWithOptions>>> GetAllQuestionWithOptionsPagedAsync(QueryParameters queryParameters);
    }
}
