using AutoMapper;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.DTOs.Responses;

namespace ExamiNation.Application.Profiles.Common
{
    public class QueryParametersToPagedResponseProfile : Profile
    {
        public QueryParametersToPagedResponseProfile()
        {
            CreateMap(typeof(QueryParameters), typeof(PagedResponse<>))
                .ForMember("PageNumber", opt => opt.MapFrom((src, dest) => ((QueryParameters)src).PageNumber))
                .ForMember("PageSize", opt => opt.MapFrom((src, dest) => ((QueryParameters)src).PageSize ))
                .ForMember("Filters", opt => opt.MapFrom((src, dest) => ((QueryParameters)src).Filters));
        }
    }
}
