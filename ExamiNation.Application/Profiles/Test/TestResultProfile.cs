using AutoMapper;
using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Domain.Entities.Test;

namespace ExamiNation.Application.Profiles.Test
{
    public class TestResultProfile : Profile
    {
        public TestResultProfile()
        {
            CreateMap<TestResult, TestResultDto>()
             .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.TestName, opt => opt.MapFrom(src => src.Test.Name)).ReverseMap();
            CreateMap<CreateTestResultDto, TestResult>()
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));
            CreateMap<EditTestResultDto, TestResult>().ReverseMap();

            CreateMap<CreateAnswerDto, SubmitAnswerDto>()
           .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId))
           .ForMember(dest => dest.SelectedOptionId, opt => opt.MapFrom(src => src.OptionId));

        }
    }
}
