namespace Learnova.Business.DTOs.Contract.Users
{
    public record UpdateUserRequest
  (
      string firstName,
      string lastName,
      string email,
      string password,
      IList<string> Roles

   );
}
