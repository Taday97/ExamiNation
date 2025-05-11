using AutoMapper;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Profiles.Common
{
    public class QueryParametersToPagedQueryOptionsProfile : Profile
    {
        public QueryParametersToPagedQueryOptionsProfile()
        {
            CreateMap(typeof(QueryParameters), typeof(PagedQueryOptions<>))
                       .ForMember("PageNumber", opt => opt.MapFrom(src => ((QueryParameters)src).PageNumber ))
                       .ForMember("PageSize", opt => opt.MapFrom(src => ((QueryParameters)src).PageSize))
                       .ForMember("Filters", opt => opt.MapFrom(src => ((QueryParameters)src).Filters))
                       .ForMember("SortBy", opt => opt.MapFrom(src => ((QueryParameters)src).SortBy))
                       .ForMember("SortDescending", opt => opt.MapFrom(src => ((QueryParameters)src).SortDescending));
        }
    }
}
