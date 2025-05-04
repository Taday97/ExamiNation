using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Answer;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IAnswerService
    {
        Task<ApiResponse<IEnumerable<AnswerDto>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<AnswerDto>>> GetAllByTestAsync(Guid testId);
        Task<ApiResponse<AnswerDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<AnswerDto>> AddAsync(CreateAnswerDto AnswerDto);
        Task<ApiResponse<AnswerDto>> Update(EditAnswerDto AnswerDto);
        Task<ApiResponse<AnswerDto>> Delete(Guid id);
    }
}
