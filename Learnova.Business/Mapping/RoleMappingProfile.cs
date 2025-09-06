using AutoMapper;
using Learnova.Business.DTOs.Contract.Roles;
using Learnova.Domain.Entities;

namespace Learnova.Business.Mapping
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<ApplicationRole, RoleResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted));
        }
    }
}
