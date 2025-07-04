using AutoMapper;
using ExamiNation.Application.DTOs.Answer;
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
using ExamiNation.Infrastructure.Repositories.Test;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ExamiNation.Application.Services.Test
{
    public class CognitiveCategoryService : ICognitiveCategoryService
    {
        private readonly ICognitiveCategoryRepository _cognitiveCategoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public CognitiveCategoryService(ICognitiveCategoryRepository cognitiveCategoryRepository, IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _cognitiveCategoryRepository = cognitiveCategoryRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<CognitiveCategoryDto>>> GetAllAsync()
        {
            var cognitiveCategories = await _cognitiveCategoryRepository.GetAllAsync();

            if (cognitiveCategories == null || !cognitiveCategories.Any())
            {
                return ApiResponse<IEnumerable<CognitiveCategoryDto>>.CreateErrorResponse("No cognitiveCategories found.");
            }

            var cognitiveCategoryDtos = _mapper.Map<IEnumerable<CognitiveCategoryDto>>(cognitiveCategories);

            return ApiResponse<IEnumerable<CognitiveCategoryDto>>.CreateSuccessResponse("CognitiveCategory retrieved successfully.", cognitiveCategoryDtos);
        }
        public async Task<ApiResponse<CognitiveCategoryDto>> GetByIdAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse("CognitiveCategory ID must be a valid GUID.");
            }
            var cognitiveCategory = await _cognitiveCategoryRepository.GetByIdAsync(guid);
            if (cognitiveCategory == null)
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse($"CognitiveCategory with id {id} not found.");
            }

            var cognitiveCategoryDto = _mapper.Map<CognitiveCategoryDto>(cognitiveCategory);
            return ApiResponse<CognitiveCategoryDto>.CreateSuccessResponse("CognitiveCategory retrieved successfully.", cognitiveCategoryDto);
        }

        public async Task<ApiResponse<CognitiveCategoryDto>> AddAsync(CreateCognitiveCategoryDto cognitiveCategoryDto)
        {
            if (cognitiveCategoryDto == null)
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse("CognitiveCategory data cannot be null.");
            }

            var cognitiveCategoryEntity = _mapper.Map<CognitiveCategory>(cognitiveCategoryDto);

            var createdCognitiveCategory = await _cognitiveCategoryRepository.AddAsync(cognitiveCategoryEntity);

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

            var cognitiveCategory = await _cognitiveCategoryRepository.GetByIdAsync(guid);
            if (cognitiveCategory == null)
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse($"CognitiveCategory with id {id} not found.");
            }

            try
            {
                var cognitiveCategoryDelete = await _cognitiveCategoryRepository.DeleteAsync(guid);

            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse(
                  "This cognitive category cannot be deleted because it is being used in another entity."
               );
            }

            var cognitiveCategoryDto = _mapper.Map<CognitiveCategoryDto>(cognitiveCategory);

            return ApiResponse<CognitiveCategoryDto>.CreateSuccessResponse("CognitiveCategory deleted successfully.", cognitiveCategoryDto);
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
            var cognitiveCategory = await _cognitiveCategoryRepository.GetByIdAsync(guid);
            if (cognitiveCategory == null)
            {
                return ApiResponse<CognitiveCategoryDto>.CreateErrorResponse($"CognitiveCategory with id {editCognitiveCategoryDto.Id} not found.");
            }

            _mapper.Map(editCognitiveCategoryDto, cognitiveCategory);

            await _cognitiveCategoryRepository.UpdateAsync(cognitiveCategory);

            CognitiveCategoryDto cognitiveCategoryDto = _mapper.Map<CognitiveCategoryDto>(cognitiveCategory);
            return ApiResponse<CognitiveCategoryDto>.CreateSuccessResponse("CognitiveCategory updated successfully.", cognitiveCategoryDto);
        }

        public async Task<ApiResponse<PagedResponse<CognitiveCategoryDto>>> GetAllPagedAsync(QueryParameters queryParameters)
        {
            var cognitiveCategoriesQuery = _mapper.Map<PagedQueryOptions<CognitiveCategory>>(queryParameters);

            var (cognitiveCategories, totalCount) = await _cognitiveCategoryRepository.GetPagedWithCountAsync(cognitiveCategoriesQuery);

            var cognitiveCategoryDtos = _mapper.Map<IEnumerable<CognitiveCategoryDto>>(cognitiveCategories);

            var result = _mapper.Map<PagedResponse<CognitiveCategoryDto>>(queryParameters);
            result.Items = cognitiveCategoryDtos;
            result.TotalCount = totalCount;

            return ApiResponse<PagedResponse<CognitiveCategoryDto>>.CreateSuccessResponse("CognitiveCategorys retrieved successfully.", result);
        }

    }
}
