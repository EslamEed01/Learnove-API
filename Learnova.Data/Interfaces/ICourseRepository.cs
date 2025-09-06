using Learnova.Domain.Entities;

namespace Learnova.Infrastructure.Interfaces
{
    public interface ICourseRepository
    {

        Task<IEnumerable<Course>> GetAllCoursesAsync(int page, int pageSize);
        Task<Course> GetCourseByIdAsync(int id);
        Task<Course> GetCoursesByCategoryAsync(int categoryId);
        Task CreateCourseAsync(Course course);
        Task<Course> DeleteCourseByIdAsync(int id);

        Task<Course> UpdateCourseByIdAsync(Course course);



    }
}
