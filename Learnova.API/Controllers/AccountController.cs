using Learnova.Business.Abstraction;
using Learnova.Business.DTOs.Contract.Users;
using Learnova.Business.Identity;
using Learnova.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learnova.API.Controllers
{
    [ApiController]
    [Route("me")]
    [Authorize]
    public class AccountController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;



        /// <summary>
        /// Get current user profile 
        /// </summary>

        [HttpGet("")]
        public async Task<IActionResult> Info()
        {
            var result = await _userService.GetProfileAsync(User.GetUserId()!);

            return Ok(result.Value);
        }


        /// <summary>
        /// info
        /// </summary>

        [HttpPut("info")]
        public async Task<IActionResult> Info([FromBody] UpdateProfileRequest request)
        {
            await _userService.UpdateProfileAsync(User.GetUserId()!, request);

            return NoContent();
        }


        /// <summary>
        /// change password
        /// </summary>

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
