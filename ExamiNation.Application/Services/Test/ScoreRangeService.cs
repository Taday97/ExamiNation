using AutoMapper;
using ExamiNation.Application.DTOs.ScoreRange;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.DTOs.ScoreRange;
using ExamiNation.Application.Interfaces.Security;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Domain.Common;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Repositories.Test;
using static System.Formats.Asn1.AsnWriter;

namespace ExamiNation.Application.Services.Test
{
    public class ScoreRangeService : IScoreRangeService
    {
        private readonly IScoreRangeRepository _scoreRangeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ScoreRangeService(IScoreRangeRepository scoreRangeRepository, IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _scoreRangeRepository = scoreRangeRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<ScoreRangeDto>>> GetAllAsync()
        {
            var scoreRanges = await _scoreRangeRepository.GetAllAsync();

            if (scoreRanges == null || !scoreRanges.Any())
            {
                return ApiResponse<IEnumerable<ScoreRangeDto>>.CreateErrorResponse("No scoreRanges found.");
            }

            var scoreRangeDtos = _mapper.Map<IEnumerable<ScoreRangeDto>>(scoreRanges);

            return ApiResponse<IEnumerable<ScoreRangeDto>>.CreateSuccessResponse("ScoreRange retrieved successfully.", scoreRangeDtos);
        }
        public async Task<ApiResponse<ScoreRangeDto>> GetByIdAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<ScoreRangeDto>.CreateErrorResponse("ScoreRange ID must be a valid GUID.");
            }
            var scoreRange = await _scoreRangeRepository.GetByIdAsync(guid);
            if (scoreRange == null)
            {
                return ApiResponse<ScoreRangeDto>.CreateErrorResponse($"ScoreRange with id {id} not found.");
            }

            var scoreRangeDto = _mapper.Map<ScoreRangeDto>(scoreRange);
            return ApiResponse<ScoreRangeDto>.CreateSuccessResponse("ScoreRange retrieved successfully.", scoreRangeDto);
        }

        public async Task<ApiResponse<ScoreRangeDto>> AddAsync(CreateScoreRangeDto scoreRangeDto)
        {
            if (scoreRangeDto == null)
            {
                return ApiResponse<ScoreRangeDto>.CreateErrorResponse("ScoreRange data cannot be null.");
            }

            var scoreRangeEntity = _mapper.Map<ScoreRange>(scoreRangeDto);

            var createdScoreRange = await _scoreRangeRepository.AddAsync(scoreRangeEntity);

            var createdScoreRangeDto = _mapper.Map<ScoreRangeDto>(createdScoreRange);

            return ApiResponse<ScoreRangeDto>.CreateSuccessResponse("ScoreRange created successfully.", createdScoreRangeDto);

        }

        public async Task<ApiResponse<ScoreRangeDto>> DeleteAsync(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out var guid))
            {
                return ApiResponse<ScoreRangeDto>.CreateErrorResponse("ScoreRange ID must be a valid GUID.");
            }
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return ApiResponse<ScoreRangeDto>.CreateErrorResponse("ScoreRange ID is required.");
            }

            var scoreRange = await _scoreRangeRepository.GetByIdAsync(guid);
            if (scoreRange == null)
            {
                return ApiResponse<ScoreRangeDto>.CreateErrorResponse($"ScoreRange with id {id} not found.");
            }

            var rolUpdateAsync = await _scoreRangeRepository.DeleteAsync(guid);

            var scoreRangeDto = _mapper.Map<ScoreRangeDto>(scoreRange);

            return ApiResponse<ScoreRangeDto>.CreateSuccessResponse("ScoreRange deleted successfully.", scoreRangeDto);
        }

        public async Task<ApiResponse<ScoreRangeDto>> UpdateAsync(EditScoreRangeDto editScoreRangeDto)
        {
            if (editScoreRangeDto == null)
            {
                return ApiResponse<ScoreRangeDto>.CreateErrorResponse("ScoreRange data cannot be null.");
            }
            if (!Guid.TryParse(editScoreRangeDto.Id.ToString(), out var guid))
            {
                return ApiResponse<ScoreRangeDto>.CreateErrorResponse("ScoreRange ID must be a valid GUID.");
            }
            var scoreRange = await _scoreRangeRepository.GetByIdAsync(guid);
            if (scoreRange == null)
            {
                return ApiResponse<ScoreRangeDto>.CreateErrorResponse($"ScoreRange with id {editScoreRangeDto.Id} not found.");
            }

            _mapper.Map(editScoreRangeDto, scoreRange);

            await _scoreRangeRepository.UpdateAsync(scoreRange);

            ScoreRangeDto scoreRangeDto = _mapper.Map<ScoreRangeDto>(scoreRange);
            return ApiResponse<ScoreRangeDto>.CreateSuccessResponse("ScoreRange updated successfully.", scoreRangeDto);
        }

        public async Task<ApiResponse<string>> GetClasificationAsync(Guid testId, int Score)
        {
            if (!Guid.TryParse(testId.ToString(), out var guid))
            {
                return ApiResponse<string>.CreateErrorResponse("ScoreRange ID must be a valid GUID.");
            }
            var scoreRange = await _scoreRangeRepository.FindFirstAsync(l => l.TestId == testId && Score > l.MinScore  &&  Score < l.MaxScore );
            if (scoreRange == null)
            {
                return ApiResponse<string>.CreateErrorResponse($"ScoreRange with id {testId.ToString()} not found.");
            }

            var scoreRangeDto = _mapper.Map<ScoreRangeDto>(scoreRange);
            return ApiResponse<string>.CreateSuccessResponse("ScoreRange retrieved successfully.", scoreRangeDto.Classification);
        }

        public async Task<ApiResponse<PagedResponse<ScoreRangeDto>>> GetAllPagedAsync(QueryParameters queryParameters)
        {
            var options = new PagedQueryOptions<ScoreRange>
            {
                Filters = queryParameters.Filters,
                SortBy = queryParameters.SortBy,
                SortDescending = queryParameters.SortDescending,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };

            var (scoreRanges, totalCount) = await _scoreRangeRepository.GetPagedWithCountAsync(options);

            if (!scoreRanges.Any())
            {
                return ApiResponse<PagedResponse<ScoreRangeDto>>.CreateErrorResponse("No scoreRanges found.");
            }

            var scoreRangeDtos = _mapper.Map<IEnumerable<ScoreRangeDto>>(scoreRanges);

            var result = new PagedResponse<ScoreRangeDto>
            {
                Items = scoreRangeDtos,
                TotalCount = totalCount,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                Filters = queryParameters.Filters,
            };

            return ApiResponse<PagedResponse<ScoreRangeDto>>.CreateSuccessResponse("ScoreRanges retrieved successfully.", result);
        }
    }
}
