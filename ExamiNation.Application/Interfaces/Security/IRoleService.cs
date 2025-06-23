using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.DTOs.Role;
using ExamiNation.Application.DTOs.User;

namespace ExamiNation.Application.Interfaces.Security
{
    public interface IRoleService
    {
        Task<ApiResponse<IEnumerable<RoleDto>>> GetAllAsync();
        Task<ApiResponse<RoleDto>> GetByIdAsync(string id);
        Task<ApiResponse<RoleDto>> AddAsync(CreateRoleDto RoleDto);
        Task<ApiResponse<RoleDto>> Update(EditRoleDto RoleDto);
        Task<ApiResponse<RoleDto>> Delete(string id);
        Task<ApiResponse<PagedResponse<RoleDto>>> GetAllPagedAsync(QueryParameters queryParameters);

    }
}
