using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Application.DTOs.TestResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface ITestResultService
    {
        Task<ApiResponse<IEnumerable<TestResultDto>>> GetAllAsync();
        Task<ApiResponse<TestResultDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<TestResultDto>> AddAsync(CreateTestResultDto TestResultDto);
        Task<ApiResponse<TestResultDto>> Update(EditTestResultDto TestResultDto);
        Task<ApiResponse<TestResultDto>> Delete(Guid id);
        Task<ApiResponse<IEnumerable<TestResultDto>>> GetByTestIdAsync(Guid testId);
    }
}
