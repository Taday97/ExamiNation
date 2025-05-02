using AutoMapper;
using ExamiNation.Application.Common.Autorization;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Application.Interfaces.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamiNation.API.Controllers.Test
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestResultController : ControllerBase
    {
        private readonly ITestResultService _testResultService;
        private readonly IMapper _mapper;

        public TestResultController(ITestResultService testResultService, IMapper mapper)
        {
            _testResultService = testResultService;
            _mapper = mapper;
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet]
        public async Task<IActionResult> GetAllTestResults()
        {
            var response = await _testResultService.GetAllAsync();

            if (!response.Success)
                return NotFound(response.Message);


            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestResultById(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                var errorResponse = ApiResponse<TestResultDto>.CreateErrorResponse("TestResult ID must be a valid GUID.");
                return BadRequest(errorResponse.Message);
            }
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest("TestResult ID is required.");
            }
            var response = await _testResultService.GetByIdAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }

        [HttpGet("by-test/{testId}")]
        public async Task<IActionResult> GetTestResultsByTest(Guid testId)
        {
            var result = await _testResultService.GetByTestIdAsync(testId);
            return Ok(result);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPost]
        public async Task<IActionResult> CreateTestResult([FromBody] CreateTestResultDto createTestResultDto)
        {
            if (createTestResultDto == null)
                return BadRequest("TestResult data cannot be null.");

            var response = await _testResultService.AddAsync(createTestResultDto);

            if (!response.Success)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(GetTestResultById), new { id = response.Data.Id }, response.Data);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTestResult(Guid id, [FromBody] EditTestResultDto editTestResultDto)
        {

            if (editTestResultDto == null)
            {
                return BadRequest("TestResult data cannot be null.");
            }
            if (editTestResultDto.Id != id)
            {
                return BadRequest("TestResult ID in the request body does not match the ID in the URL.");
            }

            editTestResultDto.Id = id;
            var response = await _testResultService.Update(editTestResultDto);
            if (!response.Success)
                return BadRequest(response.Message);
            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpDelete("{testResultId}")]
        public async Task<IActionResult> DeleteTestResult(Guid testResultId)
        {
            if (!Guid.TryParse(testResultId.ToString(), out var guid))
            {
                var errorResponse = ApiResponse<TestResultDto>.CreateErrorResponse("TestResult ID must be a valid GUID.");
                return BadRequest(errorResponse.Message);
            }
            if (string.IsNullOrEmpty(testResultId.ToString()))
            {
                var errorResponse = ApiResponse<TestResultDto>.CreateErrorResponse("TestResult ID is required.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _testResultService.Delete(testResultId);

            if (!response.Success)
                return NotFound(response.Message);
            return Ok(response);
        }


    }

}