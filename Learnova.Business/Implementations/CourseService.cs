using AutoMapper;
using Learnova.Business.DTOs.CourseDTO;
using Learnova.Business.DTOs.EnrollmentDTO;
using Learnova.Business.DTOs.EnrollmentDTO.Learnova.Business.DTOs.EnrollmentDTO;
using Learnova.Business.Services.Interfaces;
using Learnova.Domain.Entities;
using Learnova.Infrastructure.Interfaces;

namespace Learnova.Business.Implementations
{
    public class CourseService : ICourseService
    {

        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly IEnrollmentRepository _enrollmentRepository;


        public CourseService(ICourseRepository courseRepository, IMapper mapper, IEnrollmentRepository enrollmentRepository)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
            _enrollmentRepository = enrollmentRepository;
        }

        public Task AddCourseAsync(CourseDTO dto)
        {
            var course = _mapper.Map<Course>(dto);
            return _courseRepository.CreateCourseAsync(course);
        }

        public async Task<EnrollmentDTO> AddCourseMemberAsync(int courseId, CreateEnrollmentDTO dto)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            var isAlreadyEnrolled = await _enrollmentRepository.IsUserEnrolledAsync(courseId, dto.UserId);
            if (isAlreadyEnrolled)
                throw new InvalidOperationException($"User {dto.UserId} is already enrolled in course {courseId}.");

            var enrollment = new Enrollment
            {
                CourseId = courseId,
                UserId = dto.UserId,
                Status = dto.Status,
                EnrollmentDate = DateTime.UtcNow
            };

            var createdEnrollment = await _enrollmentRepository.CreateEnrollmentAsync(enrollment);
            return _mapper.Map<EnrollmentDTO>(createdEnrollment);
        }

        public async Task DeleteCourseAsync(int id)
        {
            await _courseRepository.DeleteCourseByIdAsync(id);
        }

        public async Task DeleteCourseMemberAsync(int courseId, int memberId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            var enrollment = await _enrollmentRepository.GetEnrollmentByIdAsync(memberId);
            if (enrollment == null || enrollment.CourseId != courseId)
                throw new KeyNotFoundException($"Member with ID {memberId} not found in course {courseId}.");

            await _enrollmentRepository.DeleteEnrollmentAsync(memberId);
        }

        public async Task<List<CourseDTO>> GetAllCoursesAsync(int page, int pageSize)
        {
            var courses = await _courseRepository.GetAllCoursesAsync(page, pageSize);
            return _mapper.Map<List<CourseDTO>>(courses);
        }

        public async Task<CourseDTO> GetCourseByIdAsync(int id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException($"course with ID {id} not found.");

            return _mapper.Map<CourseDTO>(course);
        }

        public async Task<List<MemberDTO>> GetCourseMembersAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            var enrollments = await _enrollmentRepository.GetCourseEnrollmentsAsync(courseId);
            return _mapper.Map<List<MemberDTO>>(enrollments);
        }

        public async Task<List<CourseDTO>> GetCoursesByCategoryAsync(int categoryId)
        {
            var courses = await _courseRepository.GetCoursesByCategoryAsync(categoryId);
            if (courses == null)
                throw new KeyNotFoundException($"No courses found for category ID {categoryId}.");
            return _mapper.Map<List<CourseDTO>>(courses);

        }

        public async Task UpdateCourseAsync(int id, CourseDTO dto)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException("Course not found");

            _mapper.Map(dto, course);
            await _courseRepository.CreateCourseAsync(course);
        }
    }
}
