namespace Learnova.Business.DTOs.Contract.Users
{
    public record ChangePasswordRequest
     (

         string CurrentPassword,
         string NewPassword


     );
}
