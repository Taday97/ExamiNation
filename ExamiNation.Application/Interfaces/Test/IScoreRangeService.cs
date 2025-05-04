using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.ScoreRange;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IScoreRangeService
    {
        Task<ApiResponse<IEnumerable<ScoreRangeDto>>> GetAllAsync();
        Task<ApiResponse<ScoreRangeDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<ScoreRangeDto>> AddAsync(CreateScoreRangeDto ScoreRangeDto);
        Task<ApiResponse<ScoreRangeDto>> Update(EditScoreRangeDto ScoreRangeDto);
        Task<ApiResponse<ScoreRangeDto>> Delete(Guid id);
        Task<ApiResponse<string>> GetClasificationAsync(Guid testId,int Score);
    }
}
