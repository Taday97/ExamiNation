using AutoMapper;
using ExamiNation.Application.DTOs.Role;
using ExamiNation.Domain.Entities.Security;

namespace ExamiNation.Application.Profiles.Security
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<CreateRoleDto, Role>();
            CreateMap<EditRoleDto, Role>().ReverseMap();
        }
    }
}
