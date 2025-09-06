using AutoMapper;
using Learnova.Business.DTOs.CourseDTO;
using Learnova.Domain.Entities;

namespace Learnova.Business.Mapping
{
    public class CourseMappingProfile : Profile
    {

        public CourseMappingProfile()
        {
            //  course
            CreateMap<Course, CourseDTO>();
            CreateMap<CourseDTO, Course>();

        }
    }
}
