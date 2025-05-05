using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Domain.Entities.Test;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IAnswerService : IGenericService<AnswerDto, CreateAnswerDto, EditAnswerDto>
    {
        Task<ApiResponse<IEnumerable<AnswerDto>>> GetAllByTestAsync(Guid testId);
    }
}
