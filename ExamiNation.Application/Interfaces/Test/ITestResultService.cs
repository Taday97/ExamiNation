using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Domain.Enums;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface ITestResultService
    {
        Task<ApiResponse<IEnumerable<TestResultDto>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<TestResultDto>>> GetAllByStatusUserIdAsync(TestResultStatus status, Guid usertId);
        Task<ApiResponse<TestResultDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<TestResultDto>> AddAsync(CreateTestResultDto TestResultDto);
        Task<ApiResponse<TestResultDto>> Update(EditTestResultDto TestResultDto);
        Task<ApiResponse<TestResultDto>> Delete(Guid id);
        Task<ApiResponse<IEnumerable<TestResultDto>>> GetByTestIdAsync(Guid testId);
        Task<ApiResponse<IEnumerable<TestResultDto>>> GetByUserIdAsync(Guid userId);
       
    }
}
