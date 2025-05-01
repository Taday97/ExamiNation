using AutoMapper;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Option;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Domain.Interfaces.Test;

namespace ExamiNation.Application.Services.Test
{
    public class OptionService : IOptionService
    {
        private readonly IOptionRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public OptionService(IOptionRepository roleRepository, IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<OptionDto>>> GetAllAsync()
        {
            var roles = await _roleRepository.GetOptionsAsync();

            if (roles == null || !roles.Any())
            {
                return ApiResponse<IEnumerable<OptionDto>>.CreateErrorResponse("No roles found.");
            }

            var roleDtos = _mapper.Map<IEnumerable<OptionDto>>(roles);

            return ApiResponse<IEnumerable<OptionDto>>.CreateSuccessResponse("Option retrieved successfully.", roleDtos);
        }
        public async Task<ApiResponse<OptionDto>> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<OptionDto>.CreateErrorResponse("Option ID must be a valid GUID.");
            }
            var role = await _roleRepository.GetByIdAsync(guid);
            if (role == null)
            {
                return ApiResponse<OptionDto>.CreateErrorResponse($"Option with id {id} not found.");
            }

            var roleDto = _mapper.Map<OptionDto>(role);
            return ApiResponse<OptionDto>.CreateSuccessResponse("Option retrieved successfully.", roleDto);
        }

        public async Task<ApiResponse<OptionDto>> AddAsync(CreateOptionDto roleDto)
        {
            if (roleDto == null)
            {
                return ApiResponse<OptionDto>.CreateErrorResponse("Option data cannot be null.");
            }

            var roleEntity = _mapper.Map<Option>(roleDto);

            var createdOption = await _roleRepository.AddAsync(roleEntity);

            var createdOptionDto = _mapper.Map<OptionDto>(createdOption);

            return ApiResponse<OptionDto>.CreateSuccessResponse("Option created successfully.", createdOptionDto);

        }

        public async Task<ApiResponse<OptionDto>> Delete(string id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<OptionDto>.CreateErrorResponse("Option ID must be a valid GUID.");
            }
            if (string.IsNullOrEmpty(id))
            {
                return ApiResponse<OptionDto>.CreateErrorResponse("Option ID is required.");
            }

            var role = await _roleRepository.GetByIdAsync(guid);
            if (role == null)
            {
                return ApiResponse<OptionDto>.CreateErrorResponse($"Option with id {id} not found.");
            }

            var rolDelete = await _roleRepository.DeleteAsync(guid);

            var roleDto = _mapper.Map<OptionDto>(role);

            return ApiResponse<OptionDto>.CreateSuccessResponse("Option deleted successfully.", roleDto);
        }

        public async Task<ApiResponse<OptionDto>> Update(EditOptionDto editOptionDto)
        {
            if (editOptionDto == null)
            {
                return ApiResponse<OptionDto>.CreateErrorResponse("Option data cannot be null.");
            }
            if (!Guid.TryParse(editOptionDto.Id.ToString(), out var guid))
            {
                return ApiResponse<OptionDto>.CreateErrorResponse("Option ID must be a valid GUID.");
            }
            var role = await _roleRepository.GetByIdAsync(guid);
            if (role == null)
            {
                return ApiResponse<OptionDto>.CreateErrorResponse($"Option with id {editOptionDto.Id} not found.");
            }

            _mapper.Map(editOptionDto, role);

            await _roleRepository.UpdateAsync(role);

            OptionDto roleDto = _mapper.Map<OptionDto>(role);
            return ApiResponse<OptionDto>.CreateSuccessResponse("Option updated successfully.", roleDto);
        }



    }
}
