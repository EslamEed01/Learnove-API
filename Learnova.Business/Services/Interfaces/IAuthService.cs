using Learnova.Business.Abstraction;
using Learnova.Business.DTOs.Contract;
using Learnova.Business.DTOs.Contract.Authentication;
using Learnova.Domain.Entities;


namespace Learnova.Business.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
        Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);


        Task<Result> RegisterAsync(DTOs.Contract.Authentication.RegisterRequest request, CancellationToken cancellationToken = default);


        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);

        Task<Result> ResendConfirmationEmailAsync(DTOs.Contract.Authentication.ResendConfirmationEmailRequest request);

        Task<Result> SendResetPasswordCodeAsync(string email);


        Task<Result> ResetPasswordAsync(DTOs.Contract.Authentication.ResetPasswordRequest request);


        Task SendConfirmationEmail(AppUser user, string code);


        Task SendResetPasswordEmail(AppUser user, string code);


        Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissions(AppUser user, CancellationToken cancellationToken);

    }
}
