using AutoMapper;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Application.Interfaces.Storage;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Enums;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Domain.Interfaces.Test;
using System.Linq.Expressions;
using TestEntity = ExamiNation.Domain.Entities.Test.Test;

namespace ExamiNation.Application.Services.Test
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IImageStorageService _imageStorage;

        public TestService(ITestRepository testRepository, IUserRepository userRepository, IUserService userService, IMapper mapper, IImageStorageService imageStorage)
        {
            _testRepository = testRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
            _imageStorage = imageStorage;
        }

        public async Task<ApiResponse<IEnumerable<TestDto>>> GetAllAsync()
        {
            var tests = await _testRepository.GetAllAsync();

            if (tests == null || !tests.Any())
            {
                return ApiResponse<IEnumerable<TestDto>>.CreateErrorResponse("No tests found.");
            }

            var testDtos = _mapper.Map<IEnumerable<TestDto>>(tests);

            return ApiResponse<IEnumerable<TestDto>>.CreateSuccessResponse("Test retrieved successfully.", testDtos);
        }
        public async Task<ApiResponse<IEnumerable<TestDto>>> GetAllByTypeAsync(TestType type)
        {
            if (!Enum.IsDefined(typeof(TestType), type))
            {
                return ApiResponse<IEnumerable<TestDto>>.CreateErrorResponse("Invalid test type provided.");
            }

            var options = new QueryOptions<TestEntity>
            {
                Filter = l => l.Type == type
            };
            var tests = await _testRepository.GetAllAsync(options);

            if (tests == null || !tests.Any())
            {
                return ApiResponse<IEnumerable<TestDto>>.CreateErrorResponse($"No tests found for type '{type}'.");
            }

            var testDtos = _mapper.Map<IEnumerable<TestDto>>(tests);

            return ApiResponse<IEnumerable<TestDto>>.CreateSuccessResponse("Tests retrieved successfully.", testDtos);
        }
        public async Task<ApiResponse<PagedResponse<TestDto>>> GetAllPagedAsync(QueryParameters queryParameters)
        {
            var options = new PagedQueryOptions<TestEntity>
            {
                Filters = queryParameters.Filters,
                SortBy = queryParameters.SortBy,
                SortDescending = queryParameters.SortDescending,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };

            var (tests, totalCount) = await _testRepository.GetPagedWithCountAsync(options);

            if (!tests.Any())
            {
                return ApiResponse<PagedResponse<TestDto>>.CreateErrorResponse("No tests found.");
            }

            var testDtos = _mapper.Map<IEnumerable<TestDto>>(tests);

            var result = new PagedResponse<TestDto>
            {
                Items = testDtos,
                TotalCount = totalCount,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                Filters = queryParameters.Filters,
            };

            return ApiResponse<PagedResponse<TestDto>>.CreateSuccessResponse("Tests retrieved successfully.", result);
        }


        public async Task<ApiResponse<TestDto>> GetByIdAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<TestDto>.CreateErrorResponse("Test ID must be a valid GUID.");
            }
            var test = await _testRepository.GetByIdAsync(guid);
            if (test == null)
            {
                return ApiResponse<TestDto>.CreateErrorResponse($"Test with id {id} not found.");
            }

            var testDto = _mapper.Map<TestDto>(test);
            return ApiResponse<TestDto>.CreateSuccessResponse("Test retrieved successfully.", testDto);
        }

        public async Task<ApiResponse<TestDto>> AddAsync(CreateTestDto testDto)
        {
            if (testDto == null)
            {
                return ApiResponse<TestDto>.CreateErrorResponse("Test data cannot be null.");
            }
            
            var testEntity = _mapper.Map<TestEntity>(testDto);

            if (testDto.ImageUrl != null && testDto.ImageUrl.Length > 0)
            {
                try
                {
                  var imageUrl = await _imageStorage.SaveImageAsync(testDto.ImageUrl, "test");

                    testEntity.ImageUrl = imageUrl;
                }
                catch (Exception ex)
                {
                    return ApiResponse<TestDto>.CreateErrorResponse($"Error while saving the image: {ex.Message}");
                }
            }


            var createdTest = await _testRepository.AddAsync(testEntity);

            var createdTestDto = _mapper.Map<TestDto>(createdTest);


            return ApiResponse<TestDto>.CreateSuccessResponse("Test created successfully.", createdTestDto);

        }

        public async Task<ApiResponse<TestDto>> DeleteAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<TestDto>.CreateErrorResponse("Test ID must be a valid GUID.");
            }
            
            var test = await _testRepository.GetByIdAsync(guid);
            if (test == null)
            {
                return ApiResponse<TestDto>.CreateErrorResponse($"Test with id {id} not found.");
            }

            var rolDelete = await _testRepository.DeleteAsync(guid);

            var testDto = _mapper.Map<TestDto>(test);

            return ApiResponse<TestDto>.CreateSuccessResponse("Test deleted successfully.", testDto);
        }

        public async Task<ApiResponse<TestDto>> UpdateAsync(EditTestDto editTestDto)
        {
            if (editTestDto == null)
            {
                return ApiResponse<TestDto>.CreateErrorResponse("Test data cannot be null.");
            }
            if (!Guid.TryParse(editTestDto.Id.ToString(), out var guid))
            {
                return ApiResponse<TestDto>.CreateErrorResponse("Test ID must be a valid GUID.");
            }
            var test = await _testRepository.GetByIdAsync(guid);
            if (test == null)
            {
                return ApiResponse<TestDto>.CreateErrorResponse($"Test with id {editTestDto.Id} not found.");
            }

            _mapper.Map(editTestDto, test);


            if (editTestDto.ImageUrl != null && editTestDto.ImageUrl.Length > 0)
            {
                try
                {
                    var imageUrl = await _imageStorage.SaveImageAsync(editTestDto.ImageUrl, "test");

                    test.ImageUrl = imageUrl;
                }
                catch (Exception ex)
                {
                    return ApiResponse<TestDto>.CreateErrorResponse($"Error while saving the image: {ex.Message}");
                }
            }

            await _testRepository.UpdateAsync(test);

            TestDto testDto = _mapper.Map<TestDto>(test);
            return ApiResponse<TestDto>.CreateSuccessResponse("Test updated successfully.", testDto);
        }



    }
}
