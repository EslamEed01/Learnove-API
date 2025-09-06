using Learnova.Business.DTOs.CourseDTO;
using Learnova.Business.DTOs.EnrollmentDTO;
using Learnova.Business.DTOs.EnrollmentDTO.Learnova.Business.DTOs.EnrollmentDTO;

namespace Learnova.Business.Services.Interfaces
{
    public interface ICourseService
    {


        Task<List<CourseDTO>> GetAllCoursesAsync(int page, int pageSize);
        Task<CourseDTO> GetCourseByIdAsync(int id);
        Task<List<CourseDTO>> GetCoursesByCategoryAsync(int categoryId);
        Task AddCourseAsync(CourseDTO dto);
        Task UpdateCourseAsync(int id, CourseDTO dto);
        Task DeleteCourseAsync(int id);

        Task<List<MemberDTO>> GetCourseMembersAsync(int courseId);
        Task<EnrollmentDTO> AddCourseMemberAsync(int courseId, CreateEnrollmentDTO dto);
        Task DeleteCourseMemberAsync(int courseId, int memberId);
    }
}
