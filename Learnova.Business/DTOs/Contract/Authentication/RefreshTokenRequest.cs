namespace Learnova.Business.DTOs.Contract.Authentication
{
    public record RefreshTokenRequest(
   string Token,
   string RefreshToken
);
}
