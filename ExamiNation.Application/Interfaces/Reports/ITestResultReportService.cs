using ExamiNation.Application.DTOs.CognitiveCategory;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Domain.Entities.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Interfaces.Reports
{
    public interface ITestResultReportService
    {
        Task<List<TestResultReportDto>> TestResultsReportsMapping(IEnumerable<TestResult> testResult);
        Task<List<CognitiveCategoryResultDto>> CalculateCategoryResults(TestResult result);
    }
}
