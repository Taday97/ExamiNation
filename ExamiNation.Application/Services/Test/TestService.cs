using AutoMapper;
using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Application.Interfaces.Storage;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Enums;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Repositories.Test;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
            var QueryOptions = new QueryOptions<TestEntity>
            {
                Includes = new List<Expression<Func<TestEntity, object>>>
                {
                   l => l.Questions,
                }
            };
            var tests = await _testRepository.GetAllAsync(QueryOptions);

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
                Filter = l => l.Type == type,
                Includes = new List<Expression<Func<TestEntity, object>>>
                {
                   l => l.Questions,
                }

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
            var optionsQuery = _mapper.Map<PagedQueryOptions<TestEntity>>(queryParameters);


            var (tests, totalCount) = await _testRepository.GetPagedWithCountAsync(optionsQuery);

            var testDtos = _mapper.Map<IEnumerable<TestDto>>(tests);

            var result = _mapper.Map<PagedResponse<TestDto>>(queryParameters);

            result.Items = testDtos;
            result.TotalCount = totalCount;

            return ApiResponse<PagedResponse<TestDto>>.CreateSuccessResponse("Tests retrieved successfully.", result);
        }


        public async Task<ApiResponse<TestDto>> GetByIdAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<TestDto>.CreateErrorResponse("Test ID must be a valid GUID.");
            }
            var test = await _testRepository.GetByIdAsync(guid, true, l => l.Include(m => m.Questions));
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

            testEntity.CreatedAt = DateTime.Now;
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

            try
            {
                var _testResultDelete = await _testRepository.DeleteAsync(guid);

            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
            {
                return ApiResponse<TestDto>.CreateErrorResponse(
                  "This test cannot be deleted because it is being used in another entity."
               );
            }

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


            if (editTestDto.ImageFile != null && editTestDto.ImageFile.Length > 0)
            {
                try
                {
                    var imageUrl = await _imageStorage.SaveImageAsync(editTestDto.ImageFile, "test");

                    test.ImageUrl = imageUrl;
                }
                catch (Exception ex)
                {
                    return ApiResponse<TestDto>.CreateErrorResponse($"Error while saving the image: {ex.Message}");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(editTestDto.ImageUrl))
                    test.ImageUrl = editTestDto.ImageUrl;
            }

            await _testRepository.UpdateAsync(test);

            TestDto testDto = _mapper.Map<TestDto>(test);
            return ApiResponse<TestDto>.CreateSuccessResponse("Test updated successfully.", testDto);
        }



    }
}
