using AutoMapper;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Auth;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Application.DTOs.User;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Security;
using ExamiNation.Domain.Enums;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Infrastructure.Helpers;
using ExamiNation.Infrastructure.Repositories;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace ExamiNation.Application.Services.Security
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly JwtService _jwtService;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper, UserManager<ApplicationUser> userManager, IConfiguration configuration, JwtService jwtService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<UserPorfileDto>> GetByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResponse<UserPorfileDto>.CreateErrorResponse($"User with id {id} not found.");
            }

            var userDto = _mapper.Map<UserPorfileDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Roles = roles.ToList();
            return ApiResponse<UserPorfileDto>.CreateSuccessResponse("User retrieved successfully.", userDto);
        }


        public async Task<ApiResponse<UserDto>> Delete(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id.ToString());
            if (user == null)
            {
                return ApiResponse<UserDto>.CreateErrorResponse($"User with id {id} not found.");
            }

            await _userRepository.DeleteAsync(id);

            var userDto = _mapper.Map<UserDto>(user);

            return ApiResponse<UserDto>.CreateSuccessResponse("User deleted successfully.", userDto);
        }


        public async Task<ApiResponse<IEnumerable<UserDto>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            if (users == null || !users.Any())
            {
                return ApiResponse<IEnumerable<UserDto>>.CreateErrorResponse("No users found.");
            }

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            return ApiResponse<IEnumerable<UserDto>>.CreateSuccessResponse("Users retrieved successfully.", userDtos);
        }


        public async Task<ApiResponse<UserDto>> Update(UserDto userDto)
        {
            if (userDto == null)
            {
                return ApiResponse<UserDto>.CreateErrorResponse("User data cannot be null.");
            }

            var user = await _userRepository.GetByIdAsync(userDto.Id.ToString());
            if (user == null)
            {
                return ApiResponse<UserDto>.CreateErrorResponse($"User with id {userDto.Id} not found.");
            }
            var existingUserWithEmail = await _userRepository.GetUsersAsync(l => (l.Email == userDto.Email || l.UserName == userDto.UserName) && l.Id != userDto.Id);
            if (existingUserWithEmail.Any())
            {
                return ApiResponse<UserDto>.CreateErrorResponse("Email or username is already in use."); ;
            }

            _mapper.Map(userDto, user);
            await _userRepository.UpdateAsync(user);

            return ApiResponse<UserDto>.CreateSuccessResponse("User updated successfully.", userDto);
        }


        public async Task<ApiResponse<string>> RegisterUser(RegisterModelDto model)
        {
            var existingEmailUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmailUser != null)
                return ApiResponse<string>.CreateErrorResponse("This email is already registered.");

            var existingUsernameUser = await _userManager.FindByNameAsync(model.Username);
            if (existingUsernameUser != null)
                return ApiResponse<string>.CreateErrorResponse("This username is already taken.");

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<string>.CreateErrorResponse("User registration failed.", errors);
            }

            var response = await AssignRolesToUserAsync(user.Id.ToString(), new List<string> { RoleEnum.User.ToString() });

            if (!response.Success)
            {
                var errors = response.Errors.ToList();
                return ApiResponse<string>.CreateErrorResponse("User registration failed.", errors);
            }

            return ApiResponse<string>.CreateSuccessResponse("User registered successfully.");
        }

        public async Task<ApiResponse<LoginResultDto>> LoginAsync(LoginModelDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return ApiResponse<LoginResultDto>.CreateErrorResponse("Invalid credentials.");
            }

            var token = await _jwtService.GenerateTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var userDto = _mapper.Map<UserLoginResponseDto>(user);
            userDto.Roles = roles;

            var result = new LoginResultDto
            {
                Token = token,
                User = userDto
            };

            return ApiResponse<LoginResultDto>.CreateSuccessResponse("Login successful.", result);
        }

        public async Task<ApiResponse<RefreshTokenModelDto>> RefreshTokenAsync(RefreshTokenModelDto model)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(model.Token);
            if (principal == null)
            {
                return ApiResponse<RefreshTokenModelDto>.CreateErrorResponse("Invalid token.");
            }

            var username = principal.Identity?.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return ApiResponse<RefreshTokenModelDto>.CreateErrorResponse("User not found.");
            }

            var newToken = await _jwtService.GenerateTokenAsync(user);
            var response = new RefreshTokenModelDto { Token = newToken };

            return ApiResponse<RefreshTokenModelDto>.CreateSuccessResponse("Token refreshed successfully.", response);
        }


        public async Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordModelDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return ApiResponse<string>.CreateErrorResponse("User not found.");

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!passwordValid)
            {
                return ApiResponse<string>.CreateErrorResponse("Old password is incorrect.");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<string>.CreateErrorResponse("Password change failed.", errors);
            }

            return ApiResponse<string>.CreateSuccessResponse("Password changed successfully.");
        }

        public async Task<ApiResponse<UserDto>> FindByNameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return ApiResponse<UserDto>.CreateErrorResponse("Username cannot be empty.");
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return ApiResponse<UserDto>.CreateErrorResponse("User not found.");
            }

            var userDto = _mapper.Map<UserDto>(user);

            return ApiResponse<UserDto>.CreateSuccessResponse("User found.", userDto);
        }

        public async Task<ApiResponse<UserPorfileDto>> GetProfileAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ApiResponse<UserPorfileDto>.CreateErrorResponse("User ID is required.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserPorfileDto>.CreateErrorResponse("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            var profileDto = _mapper.Map<UserPorfileDto>(user);
            profileDto.Roles = roles.ToList();

            return ApiResponse<UserPorfileDto>.CreateSuccessResponse("Profile retrieved successfully.", profileDto);
        }


        public async Task<ApiResponse<string>> SendResetLinkAsync(SendResetLinkModelDto model)
        {
            if (model == null)
                return ApiResponse<string>.CreateErrorResponse("SendResetLinkModelDto cannot be null.");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return ApiResponse<string>.CreateErrorResponse("Email not registered.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            //Send Email

            return ApiResponse<string>.CreateSuccessResponse("Reset link sent.");
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordModelDto model)
        {
            if (model == null)
                return ApiResponse<string>.CreateErrorResponse("ResetPasswordModelDto cannot be null.");

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return ApiResponse<string>.CreateErrorResponse("User not found.");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<string>.CreateErrorResponse("Password reset failed.", errors);
            }

            return ApiResponse<string>.CreateSuccessResponse("Password has been reset successfully.");
        }

        public async Task<ApiResponse<string>> ConfirmEmailAsync(ConfirmEmailModelDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Token))
                return ApiResponse<string>.CreateErrorResponse("Invalid request data.");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return ApiResponse<string>.CreateErrorResponse("User not found.");

            if (user.EmailConfirmed)
                return ApiResponse<string>.CreateSuccessResponse("Email already confirmed.");

            var result = await _userManager.ConfirmEmailAsync(user, model.Token);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<string>.CreateErrorResponse("Email confirmation failed.", errors);
            }

            return ApiResponse<string>.CreateSuccessResponse("Email confirmed successfully.");
        }


        public async Task<ApiResponse<bool>> AssignRolesToUserAsync(string userId, List<string> roles)
        {
            var user = await _userRepository.GetByIdAsync(userId, false);
            if (user == null)
            {
                return ApiResponse<bool>.CreateErrorResponse("User not found.");
            }

            var normalizedRoles = roles.Select(r => r.ToUpper()).ToList();
            var existingRoles = await _roleRepository.GetRolesAsync(r => normalizedRoles.Contains(r.NormalizedName.ToUpper()));
            var existingRoleNames = existingRoles.Select(r => r.NormalizedName.ToUpper()).ToList();

            if (normalizedRoles.Count != existingRoleNames.Count)
            {
                return ApiResponse<bool>.CreateErrorResponse("One or more roles not found.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var duplicateRoles = normalizedRoles.Intersect(userRoles.Select(r => r.ToUpper())).ToList();
            if (duplicateRoles.Any())
            {
                var rolesList = string.Join(", ", duplicateRoles);
                return ApiResponse<bool>.CreateErrorResponse($"The user already has the following role(s): {rolesList}.");
            }

            var result = await _userManager.AddToRolesAsync(user, roles);
            if (result.Succeeded)
            {
                return ApiResponse<bool>.CreateSuccessResponse("Roles assigned successfully.", true);
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ApiResponse<bool>.CreateErrorResponse($"Failed to assign roles: {errors}");
        }

        public async Task<ApiResponse<bool>> RemoveRolesFromUserAsync(string userId, List<string> roles)
        {
            var user = await _userRepository.GetByIdAsync(userId, false);
            if (user == null)
            {
                return ApiResponse<bool>.CreateErrorResponse("User not found.");
            }

            var normalizedRoles = roles.Select(r => r.ToUpper()).ToList();
            var existingRoles = await _roleRepository.GetRolesAsync(l => normalizedRoles.Contains(l.NormalizedName.ToUpper()));

            if (existingRoles.Count() != roles.Count)
            {
                return ApiResponse<bool>.CreateErrorResponse("One or more roles not found.");
            }

            var result = await _userManager.RemoveFromRolesAsync(user, existingRoles.Select(l => l.NormalizedName));
            if (result.Succeeded)
                return ApiResponse<bool>.CreateSuccessResponse("Roles removed successfully.", true);

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ApiResponse<bool>.CreateErrorResponse($"Failed to remove roles: {errors}");

        }

        public async Task<ApiResponse<List<string>>> GetUserRolesAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<List<string>>.CreateErrorResponse("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            return ApiResponse<List<string>>.CreateSuccessResponse("Roles retrieved successfully.", roles.ToList());
        }

        public async Task<ApiResponse<LoginResultDto>> GoogleLoginAsync(GoogleLoginDto googleLoginDto)
        {
            if (string.IsNullOrWhiteSpace(googleLoginDto.IdToken))
                return ApiResponse<LoginResultDto>.CreateErrorResponse("Invalid Google ID token.");

            var payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginDto.IdToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { "685435682005-6354qioaa2qtluergi1mrmndst7etrj1.apps.googleusercontent.com" }
            });

            var user = await _userManager.FindByEmailAsync(payload.Email);
           
            if (user == null)
            {
                var generatedUsername = await GenerateUniqueUsernameAsync(payload.Email);
                user = new ApplicationUser
                {
                    UserName = generatedUsername,
                    Email = payload.Email,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    return ApiResponse<LoginResultDto>.CreateErrorResponse($"Failed to create user: {errors}");
                }
                var response = await AssignRolesToUserAsync(user.Id.ToString(), new List<string> { RoleEnum.User.ToString() });

                if (!response.Success)
                {
                    var errors = response.Errors.ToList();
                    return ApiResponse<LoginResultDto>.CreateErrorResponse("User registration failed.", errors);
                }
            }

            var fullUser = await _userRepository.GetByIdAsync(user.Id.ToString());
            if (fullUser == null)
                return ApiResponse<LoginResultDto>.CreateErrorResponse("User not found in repository.");

            var token = await _jwtService.GenerateTokenAsync(fullUser);
            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserLoginResponseDto>(user);
            userDto.Roles = roles.ToList();

            var result = new LoginResultDto
            {
                Token = token,
                User = userDto
            };

            return ApiResponse<LoginResultDto>.CreateSuccessResponse("Login successful.", result);
        }
        private async Task<string> GenerateUniqueUsernameAsync(string email)
        {
            var baseUsername = email.Split('@')[0];
            var username = baseUsername;
            int suffix = 0;

            while (await _userManager.FindByNameAsync(username) != null && suffix < 1000)
            {
                suffix++;
                username = $"{baseUsername}{suffix}";
            }

            if (suffix >= 1000)
            {
                throw new Exception("No available usernames found. Please try again later.");
            }

            return username;
        }

        public async Task<ApiResponse<PagedResponse<UserPorfileDto>>> GetAllPagedAsync(QueryParameters queryParameters)
        {
            var optionsQuery = _mapper.Map<PagedQueryOptions<ApplicationUser>>(queryParameters);


            var (tests, totalCount, userRolesDict) = await _userRepository.GetPagedWithCountAsync(optionsQuery);

            var testDtos = _mapper.Map<IEnumerable<UserPorfileDto>>(tests);

            var result = _mapper.Map<PagedResponse<UserPorfileDto>>(queryParameters);

            result.Items = testDtos;
            result.TotalCount = totalCount;
            var userDtos = result.Items;

            foreach (var dto in userDtos)
            {
                if (Guid.TryParse(dto.Id, out var idGuid))
                {
                    dto.Roles = userRolesDict.TryGetValue(idGuid, out var roles) ? roles : new List<string>();
                }
                else
                {
                    dto.Roles = new List<string>();
                }
            }


            return ApiResponse<PagedResponse<UserPorfileDto>>.CreateSuccessResponse("Users retrieved successfully.", result);
        }

    }

}