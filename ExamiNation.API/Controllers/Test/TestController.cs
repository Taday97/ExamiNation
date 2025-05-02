using AutoMapper;
using ExamiNation.Application.Common.Autorization;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Application.Interfaces.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamiNation.API.Controllers.Test
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly IMapper _mapper;

        public TestController(ITestService testService, IMapper mapper)
        {
            _testService = testService;
            _mapper = mapper;
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet]
        public async Task<IActionResult> GetAllTests()
        {
            var response = await _testService.GetAllAsync();

            if (!response.Success)
                return NotFound(response.Message);


            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestById(Guid id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest("Test ID is required.");
            }
            var response = await _testService.GetByIdAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }


        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPost]
        public async Task<IActionResult> CreateTest([FromBody] CreateTestDto createTestDto)
        {
            if (createTestDto == null)
                return BadRequest("Test data cannot be null.");

            var response = await _testService.AddAsync(createTestDto);

            if (!response.Success)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(GetTestById), new { id = response.Data.Id }, response.Data);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTest(Guid id, [FromBody] EditTestDto editTestDto)
        {

            if (editTestDto == null)
            {
                return BadRequest("Test data cannot be null.");
            }
            if (editTestDto.Id != id)
            {
                return BadRequest("Test ID in the request body does not match the ID in the URL.");
            }

            editTestDto.Id = id;
            var response = await _testService.Update(editTestDto);
            if (!response.Success)
                return BadRequest(response.Message);
            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpDelete("{testId}")]
        public async Task<IActionResult> DeleteTest(Guid testId)
        {
            if (string.IsNullOrEmpty(testId.ToString()))
            {
                var errorResponse = ApiResponse<TestDto>.CreateErrorResponse("Test ID is required.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _testService.Delete(testId);

            if (!response.Success)
                return NotFound(response.Message);
            return Ok(response);
        }


    }

}