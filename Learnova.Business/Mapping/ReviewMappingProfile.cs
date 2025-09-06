using AutoMapper;
using Learnova.Business.DTOs.Contract.Reviews;
using Learnova.Domain.Entities;

namespace Learnova.Business.Mapping
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<AppUser, UserSummaryDto>();

            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewDate, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewId, opt => opt.Ignore())
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore());

            CreateMap<UpdateReviewDto, Review>()
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewDate, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewId, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition(
                    (src, dest, srcMember) => srcMember != null
                ));
        }
    }
}
