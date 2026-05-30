using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace FiveTalents.Tests.Integration.Helpers;

public static class JwtTokenHelper
{
    public const string TestSecret = "FiveTalentsIntegrationTestSecretKeyLongEnough!";
    public const string TestIssuer = "FiveTalents";
    public const string TestAudience = "FiveTalentsApp";

    public static string GenerateToken(string userId, string email, string? role = null)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSecret));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, email),
            new(ClaimTypes.Email, email),
        ];

        if (role != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            if (role == "SystemAdmin")
            {
                claims.Add(new Claim("system_admin", "true"));
            }
        }

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string AdminToken() =>
        GenerateToken("test-admin-id", "admin@test.local", "SystemAdmin");

    public static string MemberToken() =>
        GenerateToken("test-member-id", "member@test.local");
}
