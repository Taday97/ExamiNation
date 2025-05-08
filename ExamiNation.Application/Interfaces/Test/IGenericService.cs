using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.DTOs.Test;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IGenericService<TDto, TCreateDto, TEditDto>
         where TDto : class
         where TCreateDto : class
         where TEditDto : class
    {
        Task<ApiResponse<IEnumerable<TDto>>> GetAllAsync();
        Task<ApiResponse<PagedResponse<TDto>>> GetAllPagedAsync(QueryParameters queryParameters);
        Task<ApiResponse<TDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<TDto>> AddAsync(TCreateDto createDto);
        Task<ApiResponse<TDto>> UpdateAsync(TEditDto editDto);
        Task<ApiResponse<TDto>> DeleteAsync(Guid id);
    }
}
