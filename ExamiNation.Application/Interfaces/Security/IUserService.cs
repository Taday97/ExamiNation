using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Auth;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.DTOs.User;
using ExamiNation.Domain.Entities;

namespace ExamiNation.Application.Interfaces.Security
{
    public interface IUserService
    {
        Task<ApiResponse<IEnumerable<UserDto>>> GetAllAsync();
        Task<ApiResponse<UserPorfileDto>> GetByIdAsync(string id);
        Task<ApiResponse<UserDto>> Delete(Guid id);
        Task<ApiResponse<UserDto>> Update(UserDto userDto);

        Task<ApiResponse<string>> RegisterUser(RegisterModelDto model);
        Task<ApiResponse<LoginResultDto>> LoginAsync(LoginModelDto model);
        Task<ApiResponse<RefreshTokenModelDto>> RefreshTokenAsync(RefreshTokenModelDto model);

        Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordModelDto model);
        Task<ApiResponse<UserDto>> FindByNameAsync(string username);
        Task<ApiResponse<UserPorfileDto>> GetProfileAsync(string userId);

        Task<ApiResponse<string>> SendResetLinkAsync(SendResetLinkModelDto model);
        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordModelDto model);
        Task<ApiResponse<string>> ConfirmEmailAsync(ConfirmEmailModelDto model);
        Task<ApiResponse<List<string>>> GetUserRolesAsync(string userId);
        Task<ApiResponse<bool>> AssignRolesToUserAsync(string userId, List<string> roles);
        Task<ApiResponse<bool>> RemoveRolesFromUserAsync(string userId, List<string> roles);
        Task<ApiResponse<LoginResultDto>> GoogleLoginAsync(GoogleLoginDto idToken);
        Task<ApiResponse<PagedResponse<UserDto>>> GetAllPagedAsync(QueryParameters queryParameters);
    }
}
