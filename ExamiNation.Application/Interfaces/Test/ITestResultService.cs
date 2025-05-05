using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Domain.Enums;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface ITestResultService : IGenericService<TestResultDto, CreateTestResultDto, EditTestResultDto>
    {
        Task<ApiResponse<IEnumerable<TestResultDto>>> GetAllByStatusUserIdAsync(TestResultStatus status, Guid usertId);
        Task<ApiResponse<IEnumerable<TestResultDto>>> GetByTestIdAsync(Guid testId);
        Task<ApiResponse<IEnumerable<TestResultDto>>> GetByUserIdAsync(Guid userId);

    }
}
