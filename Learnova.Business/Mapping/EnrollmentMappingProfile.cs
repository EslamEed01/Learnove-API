using AutoMapper;
using Learnova.Business.DTOs.EnrollmentDTO;
using Learnova.Domain.Entities;

namespace Learnova.Business.Mapping
{
    public class EnrollmentMappingProfile : Profile
    {

        public EnrollmentMappingProfile()
        {

            CreateMap<Enrollment, EnrollmentDTO>();
            CreateMap<Enrollment, MemberDTO>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));



        }
    }
}
