using AutoMapper;
using ExamiNation.Application.Common.Autorization;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Option;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Enums;
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

        //[Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet]
        public async Task<IActionResult> GetAllTests()
        {
            var response = await _testService.GetAllAsync();

            if (!response.Success)
                return NotFound(response.Message);


            return Ok(response);
        }

        //[Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet("by-type/{type}")]
        public async Task<IActionResult> GetTestsByType(TestType type)
        {
            var response = await _testService.GetAllByTypeAsync(type);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response);
        }


        //[Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet("get-pages")]
        public async Task<IActionResult> GetPagedTests([FromQuery] QueryParameters queryParameters)
        {
            var response = await _testService.GetAllPagedAsync(queryParameters);

            if (!response.Success)
            {
                return NotFound(response.Message);
            }

            return Ok(response);
        }

        //[Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestById(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<OptionDto>.CreateErrorResponse("Test ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }
            var response = await _testService.GetByIdAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }


        //[Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateTest([FromQuery] CreateTestDto createTestDto)
        {
            if (createTestDto == null)
                return BadRequest("Test data cannot be null.");

            var response = await _testService.AddAsync(createTestDto);

            if (!response.Success)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(GetTestById), new { id = response.Data.Id }, response.Data);
        }

        //[Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateTest(Guid id, [FromQuery] EditTestDto editTestDto)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<OptionDto>.CreateErrorResponse("Test ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }
            if (editTestDto == null)
            {
                return BadRequest("Test data cannot be null.");
            }
            if (editTestDto.Id != id)
            {
                return BadRequest("Test ID in the request body does not match the ID in the URL.");
            }

            editTestDto.Id = id;
            var response = await _testService.UpdateAsync(editTestDto);
            if (!response.Success)
                return BadRequest(response.Message);
            return Ok(response);
        }

        //[Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTest(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<OptionDto>.CreateErrorResponse("Test ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _testService.DeleteAsync(id);

            if (!response.Success)
                return NotFound(response.Message);
            return Ok(response);
        }


    }

}