using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Auth;
using ExamiNation.Application.DTOs.User;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Domain.Entities.Security;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExamiNation.API.Controllers.Security
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userService.RegisterUser(model);

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(new { message = response.Message });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _userService.LoginAsync(model);

            if (!response.Success)
                return Unauthorized(new { message = response.Message });

            return Ok(response.Data);
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModelDto model)
        {
            var response = await _userService.RefreshTokenAsync(model);

            if (!response.Success)
                return Unauthorized(new { message = response.Message });

            return Ok(response.Data);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            var response = await _userService.GetProfileAsync(userId);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(response.Data);
        }

        [HttpGet("check-status")]
        public IActionResult CheckStatus()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized(new { message = "Not authenticated" });

            var user = new UserLoginResponseDto
            {
                Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                UserName = User.Identity?.Name,
                Roles = User.Claims
                            .Where(c => c.Type == ClaimTypes.Role)
                            .Select(c => c.Value)
                            .ToList()
            };

            var authHeader = Request.Headers["Authorization"].ToString();
            var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length) : null;

            var result = new LoginResultDto
            {
                Token = token,
                User = user
            };

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("send-reset-link")]
        public async Task<IActionResult> SendResetLink([FromBody] SendResetLinkModelDto model)
        {
            var response = await _userService.SendResetLinkAsync(model);

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response.Message);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModelDto model)
        {
            var response = await _userService.ResetPasswordAsync(model);

            if (!response.Success)
            {
                return BadRequest(new
                {
                    message = response.Message,
                    errors = response.Errors
                });
            }

            return Ok(response.Message);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModelDto model)
        {
            var response = await _userService.ChangePasswordAsync(model);

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response.Message);
        }

        [AllowAnonymous]
        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmailViaLink([FromQuery] string token, [FromQuery] string email)
        {
            var model = new ConfirmEmailModelDto { Token = token, Email = email };
            var response = await _userService.ConfirmEmailAsync(model);

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response.Message);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.IdToken))
            {
                return BadRequest(ApiResponse<string>.CreateErrorResponse("Invalid Google ID token."));
            }

            var response = await _userService.GoogleLoginAsync(dto);

            if (!response.Success)
                return Unauthorized(new { message = response.Message });

            return Ok(response.Data);
        }
       


    }
}