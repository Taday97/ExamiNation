using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Question;
using ExamiNation.Application.DTOs.Test;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IQuestionService
    {
        Task<ApiResponse<IEnumerable<QuestionDto>>> GetAllAsync();
        Task<ApiResponse<QuestionDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<QuestionDto>> AddAsync(CreateQuestionDto QuestionDto);
        Task<ApiResponse<QuestionDto>> Update(EditQuestionDto QuestionDto);
        Task<ApiResponse<QuestionDto>> Delete(Guid id);
        Task<ApiResponse<IEnumerable<QuestionDto>>> GetByTestIdAsync(Guid testId);
    }
}
