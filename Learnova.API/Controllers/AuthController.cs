using Learnova.Business.Abstraction;
using Learnova.Business.DTOs.Contract;
using Learnova.Business.DTOs.Contract.Authentication;
using Learnova.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Learnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;




        /// <summary>
        /// Authenticate user and return JWT token  
        /// </summary>

        [HttpPost("Login")]
        //[EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

            return authResult is null ? BadRequest("Invalid email/password") : Ok(authResult);
        }



        /// <summary>
        /// Refresh JWT token using refresh token  
        /// </summary>


        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

            return authResult is null ? BadRequest("Invalid token") : Ok(authResult);
        }


        /// <summary>
        /// Revoke refresh token  
        /// </summary>


        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }



        /// <summary>
        /// Register a new user  
        /// </summary>

        [HttpPost("register")]
        [EnableRateLimiting("RegisterPolicy")]

        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }


        /// <summary>
        /// Confirm email address   
        /// </summary>


        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.ConfirmEmailAsync(request);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }


        /// <summary>
        /// Resend confirmation email  
        /// </summary>


        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.ResendConfirmationEmailAsync(request);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }


        /// <summary>
        /// Send forget password code to email  
        /// </summary>


        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
        {
            var result = await _authService.SendResetPasswordCodeAsync(request.Email);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }


        /// <summary>
        /// Reset password using code sent to email  
        /// </summary>


        [HttpPost("reset-password")]
        //[EnableRateLimiting("PasswordResetPolicy")]

        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }

    }
}
