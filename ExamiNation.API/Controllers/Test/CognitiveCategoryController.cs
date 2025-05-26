using AutoMapper;
using ExamiNation.Application.Common.Autorization;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.CognitiveCategory;
using ExamiNation.Application.Interfaces.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamiNation.API.Controllers.Test
{
    [Route("api/[controller]")]
    public class CognitiveCategoryController : ControllerBase
    {
        private readonly ICognitiveCategoryService _optionService;
        private readonly IMapper _mapper;

        public CognitiveCategoryController(ICognitiveCategoryService optionService, IMapper mapper)
        {
            _optionService = optionService;
            _mapper = mapper;
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet]
        public async Task<IActionResult> GetAllCognitiveCategorys()
        {
            var response = await _optionService.GetAllAsync();

            if (!response.Success)
                return NotFound(response.Message);


            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCognitiveCategoryById(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<CognitiveCategoryDto>.CreateErrorResponse("CognitiveCategory ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _optionService.GetByIdAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }


        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPost]
        public async Task<IActionResult> CreateCognitiveCategory([FromBody] CreateCognitiveCategoryDto createCognitiveCategoryDto)
        {
            if (createCognitiveCategoryDto == null)
                return BadRequest("CognitiveCategory data cannot be null.");

            var response = await _optionService.AddAsync(createCognitiveCategoryDto);

            if (!response.Success)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(GetCognitiveCategoryById), new { id = response.Data.Id }, response.Data);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCognitiveCategory(Guid id, [FromBody] EditCognitiveCategoryDto editCognitiveCategoryDto)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<CognitiveCategoryDto>.CreateErrorResponse("CognitiveCategory ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            if (editCognitiveCategoryDto == null)
            {
                return BadRequest("CognitiveCategory data cannot be null.");
            }
            if (editCognitiveCategoryDto.Id != id)
            {
                return BadRequest("CognitiveCategory ID in the request body does not match the ID in the URL.");
            }

            editCognitiveCategoryDto.Id = id;
            var response = await _optionService.UpdateAsync(editCognitiveCategoryDto);
            if (!response.Success)
                return BadRequest(response.Message);
            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCognitiveCategory(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<CognitiveCategoryDto>.CreateErrorResponse("CognitiveCategory ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _optionService.DeleteAsync(id);

            if (!response.Success)
                return NotFound(response.Message);
            return Ok(response);
        }


    }

}