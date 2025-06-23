using AutoMapper;
using ExamiNation.Application.Common.Autorization;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.ScoreRange;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Application.Services.Test;
using ExamiNation.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamiNation.API.Controllers.Test
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoreRangeController : ControllerBase
    {
        private readonly IScoreRangeService _scoreRangeService;
        private readonly IMapper _mapper;

        public ScoreRangeController(IScoreRangeService scoreRangeService, IMapper mapper)
        {
            _scoreRangeService = scoreRangeService;
            _mapper = mapper;
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet]
        public async Task<IActionResult> GetAllScoreRanges()
        {
            var response = await _scoreRangeService.GetAllAsync();

            if (!response.Success)
                return NotFound(response.Message);


            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScoreRangeById(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<ScoreRangeDto>.CreateErrorResponse("ScoreRange ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _scoreRangeService.GetByIdAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet("classification")]
        public async Task<IActionResult> GetClassificationAsync([FromQuery] Guid testId, [FromQuery] decimal score)
        {
            if (testId == Guid.Empty)
            {
                return BadRequest("Invalid test ID.");
            }

            if (score < 0)
            {
                return BadRequest("Score must be a non-negative value.");
            }

            var result = await _scoreRangeService.GetClasificationAsync(testId, score);

            if (result == null)
            {
                return NotFound("No classification found for the given score.");
            }

            return Ok(result);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPost]
        public async Task<IActionResult> CreateScoreRange([FromBody] CreateScoreRangeDto createScoreRangeDto)
        {
            if (createScoreRangeDto == null)
                return BadRequest("ScoreRange data cannot be null.");

            var response = await _scoreRangeService.AddAsync(createScoreRangeDto);

            if (!response.Success)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(GetScoreRangeById), new { id = response.Data.Id }, response.Data);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateScoreRange(Guid id, [FromBody] EditScoreRangeDto editScoreRangeDto)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<ScoreRangeDto>.CreateErrorResponse("ScoreRange ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            if (editScoreRangeDto == null)
            {
                return BadRequest("ScoreRange data cannot be null.");
            }
            if (editScoreRangeDto.Id != id)
            {
                return BadRequest("ScoreRange ID in the request body does not match the ID in the URL.");
            }

            editScoreRangeDto.Id = id;
            var response = await _scoreRangeService.UpdateAsync(editScoreRangeDto);
            if (!response.Success)
                return BadRequest(response.Message);
            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScoreRange(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<ScoreRangeDto>.CreateErrorResponse("ScoreRange ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _scoreRangeService.DeleteAsync(id);

            if (!response.Success)
                return NotFound(response.Message);
            return Ok(response);
        }
        
        [HttpGet("pages")]
        public async Task<IActionResult> GetPagedTests([FromQuery] QueryParameters queryParameters)
        {
            var response = await _scoreRangeService.GetAllPagedAsync(queryParameters);

            if (!response.Success)
            {
                return NotFound(response.Message);
            }

            return Ok(response);
        }


    }

}
