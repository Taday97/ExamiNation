using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Test;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface ITestService: IGenericService<TestDto, CreateTestDto, EditTestDto>
    {
    }
}
