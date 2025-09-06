using AutoMapper;
using Learnova.Business.DTOs.Contract.Authentication;
using Learnova.Business.DTOs.Contract.Users;
using Learnova.Domain.Entities;

namespace Learnova.Business.Mapping
{
    public class UserMappingProfile : Profile
    {

        public UserMappingProfile()
        {
            CreateMap<AppUser, UserResponse>();
            CreateMap<AppUser, UserProfileResponse>();

            CreateMap<UserWithRoles, UserResponse>()
           .ConstructUsing(src => new UserResponse(
                   src.User.Id,
                   src.User.FirstName,
                   src.User.LastName,
                   src.User.Email!,
                   src.User.RoleType,
                   src.User.IsDisabled,
                   src.Roles.ToList()
                                     ));

            CreateMap<UpdateUserRequest, AppUser>();
            CreateMap<RegisterRequest, AppUser>();


        }
    }
}
