using AutoMapper;
using Learnova.Business.DTOs.Contract.Reviews;
using Learnova.Business.Services.Interfaces;
using Learnova.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;

        public ReviewsController(IReviewService reviewService, IMapper mapper)
        {
            _reviewService = reviewService;
            _mapper = mapper;
        }

        /// <summary>
        /// get all reviews for a product with pagination
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetReviews(int courseId, int page = 1, int pageSize = 10)
        {
            var reviews = await _reviewService.GetCourseReviewsAsync(courseId, page, pageSize);
            var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews);
            return Ok(reviewDtos);
        }

        /// <summary>
        /// get a specific review by id for a product
        /// </summary>
        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetReview(int courseId, int reviewId)
        {
            var review = await _reviewService.GetReviewAsync(reviewId);
            if (review == null || review.CourseId != courseId)
                return NotFound();

            var reviewDto = _mapper.Map<ReviewDto>(review);
            return Ok(reviewDto);
        }

        /// <summary>
        /// create a new review for a product
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview(int courseId, [FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var reviewEntity = _mapper.Map<Review>(dto);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                reviewEntity.CourseId = courseId;
                reviewEntity.ReviewDate = DateTime.UtcNow;

                var createdReview = await _reviewService.CreateReviewAsync(courseId, userId, reviewEntity);
                var createdDto = _mapper.Map<ReviewDto>(createdReview);

                return CreatedAtAction(nameof(GetReview),
                    new { courseId, reviewId = createdDto.ReviewId },
                    createdDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// update an existing review for a product
        /// </summary>
        [HttpPut("{reviewId}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int courseId, int reviewId, [FromBody] UpdateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var reviewEntity = _mapper.Map<Review>(dto);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var updatedReview = await _reviewService.UpdateReviewAsync(reviewId, userId, reviewEntity);

                if (updatedReview.CourseId != courseId)
                    return BadRequest(new { message = "Review does not belong to this product." });

                var updatedDto = _mapper.Map<ReviewDto>(updatedReview);
                return Ok(updatedDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Review not found." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        /// <summary>
        /// delete a review for a product
        /// </summary>
        [HttpDelete("{reviewId}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int courseId, int reviewId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _reviewService.DeleteReviewAsync(reviewId, userId!);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}