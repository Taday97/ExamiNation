using AutoMapper;
using ExamiNation.Application.DTOs.Question;
using ExamiNation.Application.Mapping.Resolvers;
using ExamiNation.Domain.Entities.Test;

namespace ExamiNation.Application.Profiles.Test
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<Question, QuestionDto>().ReverseMap();
            CreateMap<CreateQuestionDto, Question>()
           .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
            CreateMap<EditQuestionDto, Question>().ReverseMap();

            CreateMap<Question, QuestionDtoWithOptions>()
            .ForMember(dest => dest.SelectedOptionId, opt => opt.MapFrom<SelectedOptionResolver>())
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options)).ReverseMap();

        }
    }
}
