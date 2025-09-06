namespace Learnova.Business.DTOs.Contract.Users
{
    public class UserResponse
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string RoleType { get; set; } = default!;
        public List<string> Roles { get; set; } = [];

        public UserResponse(string id, string firstName, string lastName, string email, string roleType, bool isDisabled, List<string> roles)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            RoleType = roleType;
            isDisabled = isDisabled;
            Roles = roles;
        }
    }
}
