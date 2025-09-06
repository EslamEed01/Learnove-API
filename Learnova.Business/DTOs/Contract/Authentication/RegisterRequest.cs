namespace Learnova.Business.DTOs.Contract.Authentication
{
    public record RegisterRequest(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string UserName,
        string PhoneNumber,
        DateTime DateOfBirth,
        string? Bio
    );
}
