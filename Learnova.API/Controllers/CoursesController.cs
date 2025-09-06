using Learnova.Business.DTOs.CourseDTO;
using Learnova.Business.DTOs.EnrollmentDTO.Learnova.Business.DTOs.EnrollmentDTO;
using Learnova.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;


        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }


        /// <summary>
        /// get all courses with pagination  
        /// </summary>


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, int pageSize = 20)
        {
            var courses = await _courseService.GetAllCoursesAsync(page, pageSize);
            return Ok(courses);
        }



        /// <summary>
        /// get a specific course by id  
        /// </summary>

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            return Ok(course);
        }

        /// <summary>
        /// create a new course  
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CourseDTO dto)
        {
            await _courseService.AddCourseAsync(dto);
            return StatusCode(201);
        }


        /// <summary>
        /// update an existing course  
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CourseDTO dto)
        {
            await _courseService.UpdateCourseAsync(id, dto);
            return NoContent();
        }

        /// <summary>
        /// delete a course  
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _courseService.DeleteCourseAsync(id);
            return NoContent();
        }


        /// <summary>
        /// get all members of a course  
        /// </summary>

        [HttpGet("{id}/members")]
        [Authorize(Roles = "Admin,instructor")]
        public async Task<IActionResult> GetCourseMembers(int id)
        {
            var members = await _courseService.GetCourseMembersAsync(id);
            return Ok(members);
        }


        /// <summary>
        /// add a member to a course  
        /// </summary>

        [Authorize(Roles = "Admin,instructor")]
        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddCourseMember(int id, [FromBody] CreateEnrollmentDTO dto)
        {
            var enrollment = await _courseService.AddCourseMemberAsync(id, dto);
            return CreatedAtAction(nameof(GetCourseMembers), new { id }, enrollment);
        }


        /// <summary>
        /// remove a member from a course  
        /// </summary>



        [HttpDelete("{id}/members/{memberId}")]
        [Authorize(Roles = "Admin,instructor")]
        public async Task<IActionResult> DeleteCourseMember(int id, int memberId)
        {
            await _courseService.DeleteCourseMemberAsync(id, memberId);
            return NoContent();
        }


    }
}
