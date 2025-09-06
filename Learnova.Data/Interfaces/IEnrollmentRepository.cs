using Learnova.Domain.Entities;

namespace Learnova.Infrastructure.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<Enrollment>> GetCourseEnrollmentsAsync(int courseId);
        Task<Enrollment> GetEnrollmentByIdAsync(int enrollmentId);
        Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment);
        Task DeleteEnrollmentAsync(int enrollmentId);
        Task<bool> IsUserEnrolledAsync(int courseId, string userId);
    }
}