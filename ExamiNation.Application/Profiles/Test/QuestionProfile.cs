using AutoMapper;
using ExamiNation.Application.DTOs.Question;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Domain.Entities.Test;

namespace ExamiNation.Application.Profiles.Test
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<Question, QuestionDto>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options)).ReverseMap();
            CreateMap<CreateQuestionDto, Question>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
            CreateMap<EditQuestionDto, Question>().ReverseMap();
        }
    }
}
