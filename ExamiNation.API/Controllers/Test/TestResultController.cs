﻿using AutoMapper;
using Azure;
using ExamiNation.Application.Common.Autorization;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Option;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Application.Services.Test;
using ExamiNation.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [Authorize(Roles = RoleGroups.All)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestResultById(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<OptionDto>.CreateErrorResponse("TestResult ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }
            var response = await _testResultService.GetByIdAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }

        [Authorize(Roles = RoleGroups.All)]
        [HttpGet("test/{testId}")]
        public async Task<IActionResult> GetTestResultsByTest(Guid testId)
        {
            if (testId == Guid.Empty)
            {
                var errorResponse = ApiResponse<OptionDto>.CreateErrorResponse("Test ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }
            var result = await _testResultService.GetByTestIdAsync(testId);
            return Ok(result);
        }
        [Authorize(Roles = RoleGroups.All)]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTestResultsByUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                var errorResponse = ApiResponse<TestResultReportDto>.CreateErrorResponse("User ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }
            var result = await _testResultService.GetByUserIdAsync(userId);
            return Ok(result);
        }

        [Authorize(Roles = RoleGroups.All)]
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetTestResultsByStatus([FromRoute] int status, [FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("Invalid user ID.");
            }

            if (!Enum.IsDefined(typeof(TestResultStatus), status))
            {
                return BadRequest("Invalid status value.");
            }

            var enumStatus = (TestResultStatus)status;
            var result = await _testResultService.GetAllByStatusUserIdAsync(enumStatus, userId);
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

        [Authorize(Roles = RoleGroups.All)]
        [HttpPost("submit-answer")]
        public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerDto submitAnswerDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized(new { message = "User not authenticated or ID is invalid." });

            if (submitAnswerDto == null)
                return BadRequest("TestResult data cannot be null.");

            var response = await _testResultService.AddSubmitAnswerAsync(submitAnswerDto, userId);

            if (!response.Success)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(GetTestResultById), new { id = response.Data.Id }, response.Data);
        }

        [Authorize(Roles = RoleGroups.All)]
        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetTestResultSummary(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("The TestResult ID is invalid.");
            }

            var response = await _testResultService.GetSummaryAsync(id);

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response);
        }


        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTestResult(Guid id, [FromBody] EditTestResultDto editTestResultDto)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<OptionDto>.CreateErrorResponse("TestResult ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }
            if (editTestResultDto == null)
            {
                return BadRequest("TestResult data cannot be null.");
            }
            if (editTestResultDto.Id != id)
            {
                return BadRequest("TestResult ID in the request body does not match the ID in the URL.");
            }

            editTestResultDto.Id = id;
            var response = await _testResultService.UpdateAsync(editTestResultDto);
            if (!response.Success)
                return BadRequest(response.Message);
            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestResult(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<OptionDto>.CreateErrorResponse("TestResult ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _testResultService.DeleteAsync(id);

            if (!response.Success)
                return NotFound(response.Message);
            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet("pages")]
        public async Task<IActionResult> GetPagedTests([FromQuery] QueryParameters queryParameters)
        {
            var response = await _testResultService.GetTestResultsPagedAsync(queryParameters);

            if (!response.Success)
            {
                return NotFound(response.Message);
            }

            return Ok(response);
        }

    }

}