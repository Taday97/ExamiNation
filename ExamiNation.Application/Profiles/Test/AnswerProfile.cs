using AutoMapper;
using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.DTOs.Role;
using ExamiNation.Domain.Entities.Test;

namespace ExamiNation.Application.Profiles.Test
{
    public class AnswerProfile : Profile
    {
        public AnswerProfile()
        {
            CreateMap<Answer, AnswerDto>().ReverseMap();
            CreateMap<CreateAnswerDto, Answer>();
            CreateMap<EditAnswerDto, Answer>().ReverseMap();
        }
    }
}
