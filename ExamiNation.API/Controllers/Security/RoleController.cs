using AutoMapper;
using ExamiNation.Application.Common.Autorization;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Role;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Application.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamiNation.API.Controllers.Security
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RoleController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        [Authorize(Roles = RoleGroups.AdminOrDev)]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await _roleService.GetAllAsync();

            if (!response.Success)
                return NotFound(response.Message);


            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDev)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Role ID is required.");
            }
            var response = await _roleService.GetByIdAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }


        [Authorize(Roles = RoleGroups.AdminOrDev)]
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            if (createRoleDto == null)
                return BadRequest("Role data cannot be null.");

            var response = await _roleService.AddAsync(createRoleDto);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return CreatedAtAction(nameof(GetRoleById), new { id = response.Data.Id }, response.Data);
        }

        [Authorize(Roles = RoleGroups.AdminOrDev)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] EditRoleDto editRoleDto)
        {

            if (editRoleDto == null)
            {
                return BadRequest("Role data cannot be null.");
            }
            if (editRoleDto.Id != id)
            {
                return BadRequest("Role ID in the request body does not match the ID in the URL.");
            }

            editRoleDto.Id = id;
            var response = await _roleService.Update(editRoleDto);
            if (!response.Success)
                return BadRequest(new { message = response.Message });
            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDev)]
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                var errorResponse = ApiResponse<RoleDto>.CreateErrorResponse("Role ID is required.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _roleService.Delete(roleId);

            if (!response.Success)
                return BadRequest(new { message = response.Message });
            return Ok(response);
        }
        [HttpGet("pages")]
        public async Task<IActionResult> GetPagedTests([FromQuery] QueryParameters queryParameters)
        {
            var response = await _roleService.GetAllPagedAsync(queryParameters);

            if (!response.Success)
            {
                return NotFound(response.Message);
            }

            return Ok(response);
        }

    }
}
