using AutoMapper;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Domain.Entities.Test;

namespace ExamiNation.Application.Profiles.Test
{
    public class TestResultProfile : Profile
    {
        public TestResultProfile()
        {
            CreateMap<TestResult, TestResultDto>()
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers)).ReverseMap();
            CreateMap<CreateTestResultDto, TestResult>()
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));
            CreateMap<EditTestResultDto, TestResult>().ReverseMap();
        }
    }
}
