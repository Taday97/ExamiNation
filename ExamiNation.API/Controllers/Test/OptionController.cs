using AutoMapper;
using ExamiNation.Application.Common.Autorization;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Option;
using ExamiNation.Application.Interfaces.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamiNation.API.Controllers.Test
{
    [ApiController]
    [Route("api/[controller]")]
    public class OptionController : ControllerBase
    {
        private readonly IOptionService _optionService;
        private readonly IMapper _mapper;

        public OptionController(IOptionService optionService, IMapper mapper)
        {
            _optionService = optionService;
            _mapper = mapper;
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet]
        public async Task<IActionResult> GetAllOptions()
        {
            var response = await _optionService.GetAllAsync();

            if (!response.Success)
                return NotFound(response.Message);


            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOptionById(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<OptionDto>.CreateErrorResponse("Option ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _optionService.GetByIdAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }


        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPost]
        public async Task<IActionResult> CreateOption([FromBody] CreateOptionDto createOptionDto)
        {
            if (createOptionDto == null)
                return BadRequest("Option data cannot be null.");

            var response = await _optionService.AddAsync(createOptionDto);

            if (!response.Success)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(GetOptionById), new { id = response.Data.Id }, response.Data);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOption(Guid id, [FromBody] EditOptionDto editOptionDto)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<OptionDto>.CreateErrorResponse("Option ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            if (editOptionDto == null)
            {
                return BadRequest("Option data cannot be null.");
            }
            if (editOptionDto.Id != id)
            {
                return BadRequest("Option ID in the request body does not match the ID in the URL.");
            }

            editOptionDto.Id = id;
            var response = await _optionService.Update(editOptionDto);
            if (!response.Success)
                return BadRequest(response.Message);
            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOption(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<OptionDto>.CreateErrorResponse("Option ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _optionService.Delete(id);

            if (!response.Success)
                return NotFound(response.Message);
            return Ok(response);
        }


    }

}