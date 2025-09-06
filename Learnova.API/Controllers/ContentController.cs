using Learnova.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learnova.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContentController : ControllerBase
    {
        private readonly IPdfUploadService _pdfUploadService;
        private readonly IVideoUploadService _videoUploadService;

        /// <summary>
        /// Initializes a new instance of the ContentController.
        /// </summary>
        public ContentController(IPdfUploadService pdfUploadService, IVideoUploadService videoUploadService)
        {
            _pdfUploadService = pdfUploadService;
            _videoUploadService = videoUploadService;
        }

        /// <summary>
        /// Upload a PDF file for a specific course.  
        /// </summary>
        [HttpPost("courses/{courseId}/upload-pdf")]
        // [Authorize(Roles = "Admin,instructor")]
        public async Task<IActionResult> UploadPdf(int courseId, IFormFile file)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var result = await _pdfUploadService.UploadPdfAsync(courseId, file, currentUserId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Upload a video file for a specific lesson.  
        /// </summary>
        [HttpPost("lessons/{lessonId}/upload-video")]
        [Authorize(Roles = "Admin,instructor")]
        public async Task<IActionResult> UploadVideo(int lessonId, IFormFile file)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var result = await _videoUploadService.UploadVideoAsync(lessonId, file, currentUserId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
