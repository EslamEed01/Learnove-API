namespace Learnova.Business.DTOs.Contract.Users
{
    public record CreateUserRequest
    (
        int id,
        string firstName,
        string lastName,
        string email,
        string password,
        string RoleType,
        IList<string> Roles




    );
}
