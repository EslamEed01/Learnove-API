using Learnova.Domain.Entities;

namespace Learnova.Business.Services.Interfaces
{
    public interface IJwtProvider
    {

        public (string token, int expiresIn) GenerateToken(AppUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
        string? ValidateToken(string token);


    }
}
