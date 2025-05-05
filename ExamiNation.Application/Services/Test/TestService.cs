using AutoMapper;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Domain.Interfaces.Test;
using TestEntity = ExamiNation.Domain.Entities.Test.Test;

namespace ExamiNation.Application.Services.Test
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _optionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public TestService(ITestRepository optionRepository, IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _optionRepository = optionRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<TestDto>>> GetAllAsync()
        {
            var tests = await _optionRepository.GetAllAsync();

            if (tests == null || !tests.Any())
            {
                return ApiResponse<IEnumerable<TestDto>>.CreateErrorResponse("No tests found.");
            }

            var optionDtos = _mapper.Map<IEnumerable<TestDto>>(tests);

            return ApiResponse<IEnumerable<TestDto>>.CreateSuccessResponse("Test retrieved successfully.", optionDtos);
        }
        public async Task<ApiResponse<TestDto>> GetByIdAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<TestDto>.CreateErrorResponse("Test ID must be a valid GUID.");
            }
            var option = await _optionRepository.GetByIdAsync(guid);
            if (option == null)
            {
                return ApiResponse<TestDto>.CreateErrorResponse($"Test with id {id} not found.");
            }

            var optionDto = _mapper.Map<TestDto>(option);
            return ApiResponse<TestDto>.CreateSuccessResponse("Test retrieved successfully.", optionDto);
        }

        public async Task<ApiResponse<TestDto>> AddAsync(CreateTestDto optionDto)
        {
            if (optionDto == null)
            {
                return ApiResponse<TestDto>.CreateErrorResponse("Test data cannot be null.");
            }

            var optionEntity = _mapper.Map<TestEntity>(optionDto);

            var createdTest = await _optionRepository.AddAsync(optionEntity);

            var createdTestDto = _mapper.Map<TestDto>(createdTest);

            return ApiResponse<TestDto>.CreateSuccessResponse("Test created successfully.", createdTestDto);

        }

        public async Task<ApiResponse<TestDto>> DeleteAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<TestDto>.CreateErrorResponse("Test ID must be a valid GUID.");
            }
            
            var option = await _optionRepository.GetByIdAsync(guid);
            if (option == null)
            {
                return ApiResponse<TestDto>.CreateErrorResponse($"Test with id {id} not found.");
            }

            var rolDelete = await _optionRepository.DeleteAsync(guid);

            var optionDto = _mapper.Map<TestDto>(option);

            return ApiResponse<TestDto>.CreateSuccessResponse("Test deleted successfully.", optionDto);
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
            var option = await _optionRepository.GetByIdAsync(guid);
            if (option == null)
            {
                return ApiResponse<TestDto>.CreateErrorResponse($"Test with id {editTestDto.Id} not found.");
            }

            _mapper.Map(editTestDto, option);

            await _optionRepository.UpdateAsync(option);

            TestDto optionDto = _mapper.Map<TestDto>(option);
            return ApiResponse<TestDto>.CreateSuccessResponse("Test updated successfully.", optionDto);
        }



    }
}
