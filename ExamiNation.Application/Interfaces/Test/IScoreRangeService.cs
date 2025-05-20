using ExamiNation.Application.DTOs.ScoreRange;
using ExamiNation.Application.DTOs.ApiResponse;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IScoreRangeService : IGenericService<ScoreRangeDto, CreateScoreRangeDto, EditScoreRangeDto>
    {
        Task<ApiResponse<ScoreRangeDto>> GetClasificationAsync(Guid testId,decimal Score);
    }
}
