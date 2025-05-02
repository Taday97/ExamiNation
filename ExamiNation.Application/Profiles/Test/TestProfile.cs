using AutoMapper;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Domain.Entities.Test;
using TestEntity = ExamiNation.Domain.Entities.Test.Test;

namespace ExamiNation.Application.Profiles.Test
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<TestEntity, TestDto>().ReverseMap();
            CreateMap<CreateTestDto, TestEntity>();
            CreateMap<EditTestDto, TestEntity>().ReverseMap();
        }
    }
}
