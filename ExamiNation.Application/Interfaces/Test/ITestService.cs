using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Domain.Enums;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface ITestService : IGenericService<TestDto, CreateTestDto, EditTestDto>
    {
        Task<ApiResponse<IEnumerable<TestDto>>> GetAllByTypeAsync(TestType type);
    }
}
