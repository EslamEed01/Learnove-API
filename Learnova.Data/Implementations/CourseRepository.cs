using Learnova.Domain.Entities;
using Learnova.Infrastructure.Data.Context;
using Learnova.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Infrastructure.Implementations
{
    public class CourseRepository : ICourseRepository
    {

        private readonly LearnoveDbContext _learnoveDbContext;
        public CourseRepository(LearnoveDbContext learnoveDbContext)
        {
            _learnoveDbContext = learnoveDbContext;

        }



        public async Task CreateCourseAsync(Course course)
        {

            _learnoveDbContext.Courses.Add(course);
            await _learnoveDbContext.SaveChangesAsync();
        }

        public async Task<Course> DeleteCourseByIdAsync(int id)
        {
            await _learnoveDbContext.Courses
                 .Where(c => c.CourseId == id)
                 .ExecuteDeleteAsync();
            return await Task.FromResult(new Course { CourseId = id });
        }



        public async Task<IEnumerable<Course>> GetAllCoursesAsync(int page, int pageSize)
        {
            return await _learnoveDbContext.Courses.AsNoTracking()
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

        }

        public async Task<Course> GetCourseByIdAsync(int id)
        {
            return await _learnoveDbContext.Courses
                           .AsNoTracking()
                           .FirstOrDefaultAsync(c => c.CourseId == id);
        }



        public async Task<Course> GetCoursesByCategoryAsync(int categoryId)
        {
            return await _learnoveDbContext.Courses
                           .AsNoTracking()
                           .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        public async Task<Course> UpdateCourseByIdAsync(Course course)
        {
            var existingCourse = await _learnoveDbContext.Courses
                    .FirstOrDefaultAsync(c => c.CourseId == course.CourseId);

            await _learnoveDbContext.SaveChangesAsync();
            return existingCourse;
        }
    }
}
