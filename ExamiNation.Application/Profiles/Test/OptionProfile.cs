using AutoMapper;
using ExamiNation.Application.DTOs.Option;
using ExamiNation.Domain.Entities.Test;

namespace ExamiNation.Application.Profiles.Test
{
    public class OptionProfile : Profile
    {
        public OptionProfile()
        {
            CreateMap<Option, OptionDto>().ReverseMap();
            CreateMap<CreateOptionDto, Option>();
            CreateMap<EditOptionDto, Option>().ReverseMap();

            CreateMap<Option,QuestionOptionDto>().ReverseMap();

        }
    }
}
