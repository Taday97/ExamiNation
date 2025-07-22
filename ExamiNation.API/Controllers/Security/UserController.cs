using ExamiNation.Application.Common.Autorization;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Role;
using ExamiNation.Application.DTOs.User;
using ExamiNation.Application.Interfaces.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExamiNation.API.Controllers.Security
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found.");

            var response = await _userService.GetProfileAsync(userId);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(UserDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("User ID not found.");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest("Invalid user ID format.");

            dto.Id = userId;

            var response = await _userService.Update(dto);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response);
        }
        [Authorize(Roles = RoleGroups.AdminOrDev)]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userService.GetAllAsync();
            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }
        [Authorize(Roles = RoleGroups.AdminOrDev)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("User ID cannot be null or empty.");
            var response = await _userService.GetByIdAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }
        [Authorize(Roles = RoleGroups.AdminOrDev)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDto dto)
        {
            if (dto == null)
            {
                return BadRequest("User data cannot be null.");
            }
            if (dto.Id.ToString() != id)
            {
                return BadRequest("User ID in the request body does not match the ID in the URL.");
            }
            dto.Id = Guid.Parse( id);

            var response = await _userService.Update(dto);
            if (!response.Success)
                return BadRequest(new { message = response.Message });
            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDev)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<RoleDto>.CreateErrorResponse("User ID is required.");
                return BadRequest(errorResponse);
            }

            var response = await _userService.Delete(id);

            if (!response.Success)
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(response);
        }


        [Authorize(Roles = RoleGroups.AdminOrDev)]
        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID cannot be null or empty.");

            var response = await _userService.GetUserRolesAsync(userId);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(response);
        }
        [Authorize(Roles = RoleGroups.AdminOrDev)]
        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> AssignRolesToUser(string userId, [FromBody] List<string> roles)
        {
            if (roles == null || !roles.Any())
                return BadRequest("Users list cannot be empty.");

            var response = await _userService.AssignRolesToUserAsync(userId, roles);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDev)]
        [HttpDelete("{userId}/roles")]
        public async Task<IActionResult> RemoveRolesFromUser(string userId, [FromBody] List<string> roles)
        {
            if (roles == null || !roles.Any())
                return BadRequest("Users list cannot be empty.");

            var response = await _userService.RemoveRolesFromUserAsync(userId, roles);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDev)]
        [HttpGet("pages")]
        public async Task<IActionResult> GetPagedTests([FromQuery] QueryParameters queryParameters)
        {
            var response = await _userService.GetAllPagedAsync(queryParameters);

            if (!response.Success)
            {
                return NotFound(response.Message);
            }

            return Ok(response);
        }

    }
}
