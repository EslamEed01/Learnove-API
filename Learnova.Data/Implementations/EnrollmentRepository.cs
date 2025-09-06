using Learnova.Domain.Entities;
using Learnova.Infrastructure.Data.Context;
using Learnova.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Infrastructure.Implementations
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly LearnoveDbContext _context;

        public EnrollmentRepository(LearnoveDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Enrollment>> GetCourseEnrollmentsAsync(int courseId)
        {
            return await _context.Enrollments
                .Include(e => e.User)
                .Where(e => e.CourseId == courseId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Enrollment> GetEnrollmentByIdAsync(int enrollmentId)
        {
            return await _context.Enrollments
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);
        }

        public async Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task DeleteEnrollmentAsync(int enrollmentId)
        {
            await _context.Enrollments
                .Where(e => e.EnrollmentId == enrollmentId)
                .ExecuteDeleteAsync();
        }

        public async Task<bool> IsUserEnrolledAsync(int courseId, string userId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.CourseId == courseId && e.UserId == userId);
        }
    }
}