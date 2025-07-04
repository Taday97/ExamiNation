using ExamiNation.Application.DTOs.ScoreRange;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Question;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IScoreRangeService : IGenericService<ScoreRangeDto, CreateScoreRangeDto, EditScoreRangeDto>
    {
        Task<ApiResponse<IEnumerable<ScoreRangeDto>>> GetByTestIdAsync(Guid testId);
        Task<ApiResponse<ScoreRangeDto>> GetClasificationAsync(Guid testId,decimal Score);
    }
}
