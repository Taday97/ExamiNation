using AutoMapper;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.DTOs.Test;
using ExamiNation.Application.Mapping.Resolvers;
using ExamiNation.Domain.Common;
using ExamiNation.Infrastructure.Extensions;
using TestEntity = ExamiNation.Domain.Entities.Test.Test;

namespace ExamiNation.Application.Profiles.Test
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<TestEntity, TestDto>()
            .ReverseMap();
            CreateMap<CreateTestDto, TestEntity>();
            CreateMap<EditTestDto, TestEntity>().ReverseMap();



            CreateMap<IEnumerable<TestEntity>, PagedResponse<TestDto>>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.TotalCount, opt => opt.MapFrom(src => src.Count()));
        }
    }
}
