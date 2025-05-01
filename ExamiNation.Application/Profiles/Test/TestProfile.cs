using AutoMapper;
using ExamiNation.Application.DTOs.Option;
using ExamiNation.Domain.Entities.Test;

namespace ExamiNation.Application.Profiles.Test
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<Option, OptionDto>().ReverseMap();
            CreateMap<CreateOptionDto, Option>();
            CreateMap<EditOptionDto, Option>().ReverseMap();
        }
    }
}
