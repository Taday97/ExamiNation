using AutoMapper;
using ExamiNation.Application.DTOs.CognitiveCategory;
using ExamiNation.Domain.Entities.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Profiles.Test
{
    public class CognitiveCategoryProfile : Profile
    {
        public CognitiveCategoryProfile()
        {
            CreateMap<CognitiveCategory, CognitiveCategoryDto>().ReverseMap();
            CreateMap<CreateCognitiveCategoryDto, CognitiveCategory>();
            CreateMap<EditCognitiveCategoryDto, CognitiveCategory>().ReverseMap();

        }
    }
}