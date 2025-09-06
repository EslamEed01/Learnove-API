using AutoMapper;
using Learnova.Business.DTOs.Contract.Authentication;
using Learnova.Domain.Entities;

namespace Learnova.Business.Mapping
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<RegisterRequest, AppUser>();

        }

    }
}
