using AutoMapper;
using ExamiNation.Application.DTOs.ScoreRange;
using ExamiNation.Domain.Entities.Test;

namespace ExamiNation.Application.Profiles.Test
{
    public class ScoreRangeProfile : Profile
    {
        public ScoreRangeProfile()
        {
            CreateMap<ScoreRange, ScoreRangeDto>().ReverseMap();
            CreateMap<CreateScoreRangeDto, ScoreRange>();
            CreateMap<EditScoreRangeDto, ScoreRange>().ReverseMap();


        }
    }
}
