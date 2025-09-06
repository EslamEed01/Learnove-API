namespace Learnova.Business.DTOs.Contract.Authentication
{
    public record LoginRequest
     (
         string Email,
         string Password
     );
}
