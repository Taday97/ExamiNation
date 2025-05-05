using ExamiNation.Application.DTOs.Option;

namespace ExamiNation.Application.Interfaces.Test
{
    public interface IOptionService : IGenericService<OptionDto, CreateOptionDto, EditOptionDto>
    {
    }
}
