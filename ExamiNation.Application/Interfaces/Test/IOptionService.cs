using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Option;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IOptionService
    {
        Task<ApiResponse<IEnumerable<OptionDto>>> GetAllAsync();
        Task<ApiResponse<OptionDto>> GetByIdAsync(string id);
        Task<ApiResponse<OptionDto>> AddAsync(CreateOptionDto OptionDto);
        Task<ApiResponse<OptionDto>> Update(EditOptionDto OptionDto);
        Task<ApiResponse<OptionDto>> Delete(string id);
    }
}
