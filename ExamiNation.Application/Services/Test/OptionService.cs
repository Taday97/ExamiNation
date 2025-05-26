using AutoMapper;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Option;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Domain.Interfaces.Test;

namespace ExamiNation.Application.Services.Test
{
    public class OptionService : IOptionService
    {
        private readonly IOptionRepository _optionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public OptionService(IOptionRepository optionRepository, IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _optionRepository = optionRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<OptionDto>>> GetAllAsync()
        {
            var options = await _optionRepository.GetAllAsync();

            if (options == null || !options.Any())
            {
                return ApiResponse<IEnumerable<OptionDto>>.CreateErrorResponse("No options found.");
            }

            var optionDtos = _mapper.Map<IEnumerable<OptionDto>>(options);

            return ApiResponse<IEnumerable<OptionDto>>.CreateSuccessResponse("Option retrieved successfully.", optionDtos);
        }
        public async Task<ApiResponse<OptionDto>> GetByIdAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<OptionDto>.CreateErrorResponse("Option ID must be a valid GUID.");
            }
            var option = await _optionRepository.GetByIdAsync(guid);
            if (option == null)
            {
                return ApiResponse<OptionDto>.CreateErrorResponse($"Option with id {id} not found.");
            }

            var optionDto = _mapper.Map<OptionDto>(option);
            return ApiResponse<OptionDto>.CreateSuccessResponse("Option retrieved successfully.", optionDto);
        }

        public async Task<ApiResponse<OptionDto>> AddAsync(CreateOptionDto optionDto)
        {
            if (optionDto == null)
            {
                return ApiResponse<OptionDto>.CreateErrorResponse("Option data cannot be null.");
            }

            var optionEntity = _mapper.Map<Option>(optionDto);

            var createdOption = await _optionRepository.AddAsync(optionEntity);

            var createdOptionDto = _mapper.Map<OptionDto>(createdOption);

            return ApiResponse<OptionDto>.CreateSuccessResponse("Option created successfully.", createdOptionDto);

        }

        public async Task<ApiResponse<OptionDto>> DeleteAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<OptionDto>.CreateErrorResponse("Option ID must be a valid GUID.");
            }
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return ApiResponse<OptionDto>.CreateErrorResponse("Option ID is required.");
            }

            var option = await _optionRepository.GetByIdAsync(guid);
            if (option == null)
            {
                return ApiResponse<OptionDto>.CreateErrorResponse($"Option with id {id} not found.");
            }

            var rolDelete = await _optionRepository.DeleteAsync(guid);

            var optionDto = _mapper.Map<OptionDto>(option);

            return ApiResponse<OptionDto>.CreateSuccessResponse("Option deleted successfully.", optionDto);
        }

        public async Task<ApiResponse<OptionDto>> UpdateAsync(EditOptionDto editOptionDto)
        {
            if (editOptionDto == null)
            {
                return ApiResponse<OptionDto>.CreateErrorResponse("Option data cannot be null.");
            }
            if (!Guid.TryParse(editOptionDto.Id.ToString(), out var guid))
            {
                return ApiResponse<OptionDto>.CreateErrorResponse("Option ID must be a valid GUID.");
            }
            var option = await _optionRepository.GetByIdAsync(guid);
            if (option == null)
            {
                return ApiResponse<OptionDto>.CreateErrorResponse($"Option with id {editOptionDto.Id} not found.");
            }

            _mapper.Map(editOptionDto, option);

            await _optionRepository.UpdateAsync(option);

            OptionDto optionDto = _mapper.Map<OptionDto>(option);
            return ApiResponse<OptionDto>.CreateSuccessResponse("Option updated successfully.", optionDto);
        }

        public async Task<ApiResponse<PagedResponse<OptionDto>>> GetAllPagedAsync(QueryParameters queryParameters)
        {
            var optionsQuery = _mapper.Map<PagedQueryOptions<Option>>(queryParameters);

            var (options, totalCount) = await _optionRepository.GetPagedWithCountAsync(optionsQuery);

            if (!options.Any())
            {
                return ApiResponse<PagedResponse<OptionDto>>.CreateErrorResponse("No options found.");
            }

            var optionDtos = _mapper.Map<IEnumerable<OptionDto>>(options);

            var result = _mapper.Map<PagedResponse<OptionDto>>(queryParameters);
            result.Items = optionDtos;
            result.TotalCount = totalCount;

            return ApiResponse<PagedResponse<OptionDto>>.CreateSuccessResponse("Options retrieved successfully.", result);
        }

    }
}
