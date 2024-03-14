using AutoMapper;
using Common.Responses.Identity;
using Infrastructure.Models;

namespace Infrastructure
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<ApplicationUser, UserResponse>();
            CreateMap<ApplicationRole, RoleResponse>();
            CreateMap<ApplicationRoleClaim, RoleClaimViewModel>()
                .ForMember(x => x.Description, cd => cd.MapFrom(map => map.Descrption ?? string.Empty));
            CreateMap<RoleClaimViewModel, ApplicationRoleClaim>()
                .ForMember(dest => dest.Descrption, opt => opt.MapFrom(src => src.Description ?? string.Empty));
        }
    }
}