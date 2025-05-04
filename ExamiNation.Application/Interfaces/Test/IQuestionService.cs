using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Question;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IQuestionService
    {
        Task<ApiResponse<IEnumerable<QuestionDto>>> GetAllAsync();
        Task<ApiResponse<QuestionDtoWithOptions>> GetByIdAsync(Guid id);
        Task<ApiResponse<QuestionDto>> AddAsync(CreateQuestionDto QuestionDto);
        Task<ApiResponse<QuestionDto>> Update(EditQuestionDto QuestionDto);
        Task<ApiResponse<QuestionDto>> Delete(Guid id);
        Task<ApiResponse<IEnumerable<QuestionDto>>> GetByTestIdAsync(Guid testId);
    }
}
