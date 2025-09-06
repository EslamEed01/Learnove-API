using Learnova.Domain.Entities;

namespace Learnova.Business.DTOs.Contract.Users
{
    public class UserWithRoles
    {
        public AppUser User { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
