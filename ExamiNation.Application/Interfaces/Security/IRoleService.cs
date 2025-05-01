using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Role;

namespace ExamiNation.Application.Interfaces.Security
{
    public interface IRoleService
    {
        Task<ApiResponse<IEnumerable<RoleDto>>> GetAllAsync();
        Task<ApiResponse<RoleDto>> GetByIdAsync(string id);
        Task<ApiResponse<RoleDto>> AddAsync(CreateRoleDto RoleDto);
        Task<ApiResponse<RoleDto>> Update(EditRoleDto RoleDto);
        Task<ApiResponse<RoleDto>> Delete(string id);
    }
}
