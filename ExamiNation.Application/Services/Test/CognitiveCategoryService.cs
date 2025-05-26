using AutoMapper;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.CognitiveCategory;
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
    public class CognitiveCategoryService : ICognitiveCategoryService
    {
        private readonly ICognitiveCategoryRepository _optionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public CognitiveCategoryService(ICognitiveCategoryRepository optionRepository, IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _optionRepository = optionRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<CognitiveCategoryDto>>> GetAllAsync()
        {
            var cognitiveCategories = await _optionRepository.GetAllAsync();

            if (cognitiveCategories == null || !cognitiveCategories.Any())
            {
                return ApiResponse<IEnumerable<CognitiveCategoryDto>>.CreateErrorResponse("No cognitiveCategories found.");
            }

            var optionDtos = _mapper.Map<IEnumerable<CognitiveCategoryDto>>(cognitiveCategories);

            return ApiResponse<IEnumerable<CognitiveCategoryDto>>.CreateSuccessResponse("CognitiveCategory retrieved successfully.", optionDtos);
        }
        public async Task<ApiResponse<CognitiveCategoryDto>> GetByIdAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse("CognitiveCategory ID must be a valid GUID.");
            }
            var option = await _optionRepository.GetByIdAsync(guid);
            if (option == null)
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse($"CognitiveCategory with id {id} not found.");
            }

            var optionDto = _mapper.Map<CognitiveCategoryDto>(option);
            return ApiResponse<CognitiveCategoryDto>.CreateSuccessResponse("CognitiveCategory retrieved successfully.", optionDto);
        }

        public async Task<ApiResponse<CognitiveCategoryDto>> AddAsync(CreateCognitiveCategoryDto optionDto)
        {
            if (optionDto == null)
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse("CognitiveCategory data cannot be null.");
            }

            var optionEntity = _mapper.Map<CognitiveCategory>(optionDto);

            var createdCognitiveCategory = await _optionRepository.AddAsync(optionEntity);

            var createdCognitiveCategoryDto = _mapper.Map<CognitiveCategoryDto>(createdCognitiveCategory);

            return ApiResponse<CognitiveCategoryDto>.CreateSuccessResponse("CognitiveCategory created successfully.", createdCognitiveCategoryDto);

        }

        public async Task<ApiResponse<CognitiveCategoryDto>> DeleteAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse("CognitiveCategory ID must be a valid GUID.");
            }
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse("CognitiveCategory ID is required.");
            }

            var option = await _optionRepository.GetByIdAsync(guid);
            if (option == null)
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse($"CognitiveCategory with id {id} not found.");
            }

            var rolDelete = await _optionRepository.DeleteAsync(guid);

            var optionDto = _mapper.Map<CognitiveCategoryDto>(option);

            return ApiResponse<CognitiveCategoryDto>.CreateSuccessResponse("CognitiveCategory deleted successfully.", optionDto);
        }

        public async Task<ApiResponse<CognitiveCategoryDto>> UpdateAsync(EditCognitiveCategoryDto editCognitiveCategoryDto)
        {
            if (editCognitiveCategoryDto == null)
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse("CognitiveCategory data cannot be null.");
            }
            if (!Guid.TryParse(editCognitiveCategoryDto.Id.ToString(), out var guid))
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse("CognitiveCategory ID must be a valid GUID.");
            }
            var option = await _optionRepository.GetByIdAsync(guid);
            if (option == null)
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse($"CognitiveCategory with id {editCognitiveCategoryDto.Id} not found.");
            }

            _mapper.Map(editCognitiveCategoryDto, option);

            await _optionRepository.UpdateAsync(option);

            CognitiveCategoryDto optionDto = _mapper.Map<CognitiveCategoryDto>(option);
            return ApiResponse<CognitiveCategoryDto>.CreateSuccessResponse("CognitiveCategory updated successfully.", optionDto);
        }

        public async Task<ApiResponse<PagedResponse<CognitiveCategoryDto>>> GetAllPagedAsync(QueryParameters queryParameters)
        {
            var cognitiveCategoriesQuery = _mapper.Map<PagedQueryOptions<CognitiveCategory>>(queryParameters);

            var (cognitiveCategories, totalCount) = await _optionRepository.GetPagedWithCountAsync(cognitiveCategoriesQuery);

            if (!cognitiveCategories.Any())
            {
                return ApiResponse<PagedResponse<CognitiveCategoryDto>>.CreateErrorResponse("No cognitiveCategories found.");
            }

            var optionDtos = _mapper.Map<IEnumerable<CognitiveCategoryDto>>(cognitiveCategories);

            var result = _mapper.Map<PagedResponse<CognitiveCategoryDto>>(queryParameters);
            result.Items = optionDtos;
            result.TotalCount = totalCount;

            return ApiResponse<PagedResponse<CognitiveCategoryDto>>.CreateSuccessResponse("CognitiveCategorys retrieved successfully.", result);
        }

    }
}
