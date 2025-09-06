using Learnova.Business.Services.Interfaces;
using Learnova.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;



namespace Learnova.Infrastructure.Authentication
{
    public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
    {
        private readonly JwtOptions _options = options.Value;

        public (string token, int expiresIn) GenerateToken(AppUser user, IEnumerable<string> roles, IEnumerable<string> permissions)
        {
            Claim[] claims = [
                new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Id),
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, user.Email!),
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new("userType", user.RoleType),
            new(nameof(roles), JsonSerializer.Serialize(roles), System.IdentityModel.Tokens.Jwt.JsonClaimValueTypes.JsonArray),
            new(nameof(permissions), JsonSerializer.Serialize(permissions), System.IdentityModel.Tokens.Jwt.JsonClaimValueTypes.JsonArray)
            ];

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

            var singingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
                signingCredentials: singingCredentials
            );

            return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: _options.ExpiryMinutes * 60);
        }

        public string? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = symmetricSecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                return jwtToken.Claims.First(x => x.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub).Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
