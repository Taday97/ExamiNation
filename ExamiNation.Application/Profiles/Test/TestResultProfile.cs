using AutoMapper;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Domain.Entities.Test;

namespace ExamiNation.Application.Profiles.Test
{
    public class TestResultProfile : Profile
    {
        public TestResultProfile()
        {
            CreateMap<TestResult, TestResultDto>()
             .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Test, opt => opt.MapFrom(src => src.Test))
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers))
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers)).ReverseMap();
            CreateMap<CreateTestResultDto, TestResult>()
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));
            CreateMap<EditTestResultDto, TestResult>().ReverseMap();
        }
    }
}
