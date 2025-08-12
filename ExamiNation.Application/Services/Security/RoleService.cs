using AutoMapper;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.DTOs.Role;
using ExamiNation.Application.DTOs.User;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Security;
using ExamiNation.Domain.Interfaces.Security;
using Microsoft.EntityFrameworkCore;

namespace ExamiNation.Application.Services.Security
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<RoleDto>>> GetAllAsync()
        {
            var roles = await _roleRepository.GetRolesAsync();

            if (roles == null || !roles.Any())
            {
                return ApiResponse<IEnumerable<RoleDto>>.CreateErrorResponse("No roles found.");
            }

            var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);

            return ApiResponse<IEnumerable<RoleDto>>.CreateSuccessResponse("Role retrieved successfully.", roleDtos);
        }
        public async Task<ApiResponse<RoleDto>> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<RoleDto>.CreateErrorResponse("Role ID must be a valid GUID.");
            }
            var role = await _roleRepository.GetByIdAsync(guid);
            if (role == null)
            {
                return ApiResponse<RoleDto>.CreateErrorResponse($"Role with id {id} not found.");
            }

            var roleDto = _mapper.Map<RoleDto>(role);
            return ApiResponse<RoleDto>.CreateSuccessResponse("Role retrieved successfully.", roleDto);
        }

        public async Task<ApiResponse<RoleDto>> AddAsync(CreateRoleDto roleDto)
        {
            if (roleDto == null)
            {
                return ApiResponse<RoleDto>.CreateErrorResponse("Role data cannot be null.");
            }
            var role = await _roleRepository.FindFirstRoleAsync(l => l.NormalizedName == roleDto.Name);
            if (role != null)
            {
                return ApiResponse<RoleDto>.CreateErrorResponse($"A role with the name '{roleDto.Name}' already exists.");
            }

            var roleEntity = _mapper.Map<Role>(roleDto);

            var createdRole = await _roleRepository.AddAsync(roleEntity);

            var createdRoleDto = _mapper.Map<RoleDto>(createdRole);

            return ApiResponse<RoleDto>.CreateSuccessResponse("Role created successfully.", createdRoleDto);

        }

        public async Task<ApiResponse<RoleDto>> Delete(string id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<RoleDto>.CreateErrorResponse("Role ID must be a valid GUID.");
            }
            if (string.IsNullOrEmpty(id))
            {
                return ApiResponse<RoleDto>.CreateErrorResponse("Role ID is required.");
            }

            var role = await _roleRepository.GetByIdAsync(guid);
            if (role == null)
            {
                return ApiResponse<RoleDto>.CreateErrorResponse($"Role with id {id} not found.");
            }

            var rolDelete = await _roleRepository.DeleteAsync(guid);

            var roleDto = _mapper.Map<RoleDto>(role);

            return ApiResponse<RoleDto>.CreateSuccessResponse("Role deleted successfully.", roleDto);
        }

        public async Task<ApiResponse<RoleDto>> Update(EditRoleDto editRoleDto)
        {
            if (editRoleDto == null)
            {
                return ApiResponse<RoleDto>.CreateErrorResponse("Role data cannot be null.");
            }
            if (!Guid.TryParse(editRoleDto.Id.ToString(), out var guid))
            {
                return ApiResponse<RoleDto>.CreateErrorResponse("Role ID must be a valid GUID.");
            }
            var role = await _roleRepository.GetByIdAsync(guid);
            if (role == null)
            {
                return ApiResponse<RoleDto>.CreateErrorResponse($"Role with id {editRoleDto.Id} not found.");
            }

            var existingRole = await _roleRepository.FindFirstRoleAsync(r => r.Name == editRoleDto.Name && r.Id != guid);
            if (existingRole != null)
            {
                return ApiResponse<RoleDto>.CreateErrorResponse($"A role with the name '{editRoleDto.Name}' already exists.");
            }

            _mapper.Map(editRoleDto, role);

            await _roleRepository.UpdateAsync(role);

            RoleDto roleDto = _mapper.Map<RoleDto>(role);
            return ApiResponse<RoleDto>.CreateSuccessResponse("Role updated successfully.", roleDto);
        }

        public async Task<ApiResponse<PagedResponse<RoleDto>>> GetAllPagedAsync(QueryParameters queryParameters)
        {
            var optionsQuery = _mapper.Map<PagedQueryOptions<Role>>(queryParameters);


            var (roles, totalCount, userRolesDict) = await _roleRepository.GetPagedWithCountAsync(optionsQuery);

            var rolesDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);

            var result = _mapper.Map<PagedResponse<RoleDto>>(queryParameters);


            result.Items = rolesDtos;
            result.TotalCount = totalCount;
            var userDtos = result.Items;

            foreach (var dto in userDtos)
            {
                if (Guid.TryParse(dto.Id, out var idGuid))
                {
                    dto.UserCount = userRolesDict.TryGetValue(idGuid, out var users) ? users.Count : 0;
                }
                else
                {
                    dto.UserCount = 0;
                }
            }

            return ApiResponse<PagedResponse<RoleDto>>.CreateSuccessResponse("Tests retrieved successfully.", result);
        }

    }
}
