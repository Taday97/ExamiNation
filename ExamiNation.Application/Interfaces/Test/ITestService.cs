using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Test;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface ITestService
    {
        Task<ApiResponse<IEnumerable<TestDto>>> GetAllAsync();
        Task<ApiResponse<TestDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<TestDto>> AddAsync(CreateTestDto TestDto);
        Task<ApiResponse<TestDto>> Update(EditTestDto TestDto);
        Task<ApiResponse<TestDto>> Delete(Guid id);
    }
}
