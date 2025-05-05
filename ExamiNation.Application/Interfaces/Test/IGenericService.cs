using ExamiNation.Application.DTOs.ApiResponse;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IGenericService<TDto, TCreateDto, TEditDto>
         where TDto : class
         where TCreateDto : class
         where TEditDto : class
    {
        Task<ApiResponse<IEnumerable<TDto>>> GetAllAsync();
        Task<ApiResponse<TDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<TDto>> AddAsync(TCreateDto createDto);
        Task<ApiResponse<TDto>> UpdateAsync(TEditDto editDto);
        Task<ApiResponse<TDto>> DeleteAsync(Guid id);
    }
}
