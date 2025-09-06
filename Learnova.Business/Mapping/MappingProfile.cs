using AutoMapper;
using Learnova.Business.DTOs.CateDTO;
using Learnova.Business.DTOs.CourseDTO;
using Learnova.Domain.Entities;

namespace Learnova.Business.Mapping
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            // category
            CreateMap<Category, CategoryDTO>().ReverseMap();
            //  course
            CreateMap<Course, CourseDTO>();
            CreateMap<CourseDTO, Course>();

        }
    }
}
