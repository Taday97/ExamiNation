using AutoMapper;
using ExamiNation.Application.DTOs.User;
using ExamiNation.Domain.Entities.Security;

namespace ExamiNation.Application.Profiles.Security
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
            CreateMap<ApplicationUser, UserUpdateDto>().ReverseMap();
            CreateMap<ApplicationUser, UserPorfileDto>().ReverseMap();

            CreateMap<ApplicationUser, UserLoginResponseDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore()).ReverseMap();
        }
    }
}
