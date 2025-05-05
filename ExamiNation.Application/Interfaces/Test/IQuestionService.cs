using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Question;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IQuestionService : IGenericService<QuestionDto, CreateQuestionDto, EditQuestionDto>
    {
        Task<ApiResponse<IEnumerable<QuestionDto>>> GetByTestIdAsync(Guid testId);
    }
}
