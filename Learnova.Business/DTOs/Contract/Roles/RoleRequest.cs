using System.ComponentModel.DataAnnotations;

namespace Learnova.Business.DTOs.Contract.Roles
{
    public record RoleRequest
    (
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(256, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 256 characters")]
        string Name,

        [Required(ErrorMessage = "At least one permission is required")]
        [MinLength(1, ErrorMessage = "At least one permission must be assigned")]
        IList<string> Permissions
    );
}
