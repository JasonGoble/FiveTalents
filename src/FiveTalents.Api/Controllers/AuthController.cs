using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Infrastructure.Identity;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FiveTalents.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    IApplicationDbContext db,
    IOrganizationHierarchyService hierarchyService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        string token = await GenerateJwtTokenAsync(user);
        user.LastLoginAt = DateTime.UtcNow;
        await userManager.UpdateAsync(user);

        return Ok(new { token, user = await BuildUserResponseAsync(user) });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        ApplicationUser user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("change-password")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new { message = "Password changed successfully" });
    }

    [HttpPost("setup-account")]
    [AllowAnonymous]
    public async Task<IActionResult> SetupAccount([FromBody] SetupAccountRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return BadRequest(new { message = "Invalid request" });
        }

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        user.IsActive = true;
        await userManager.UpdateAsync(user);

        string token = await GenerateJwtTokenAsync(user);
        return Ok(new { token, user = await BuildUserResponseAsync(user) });
    }

    private async Task<object> BuildUserResponseAsync(ApplicationUser user) => new
    {
        user.Id,
        user.Email,
        user.FullName,
        user.PrimaryOrganizationId,
        user.MemberId,
        IsSystemAdmin = await userManager.IsInRoleAsync(user, "SystemAdmin")
    };

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new("full_name", user.FullName),
        };

        if (user.PrimaryOrganizationId.HasValue)
        {
            claims.Add(new("organization_id", user.PrimaryOrganizationId.Value.ToString()));
        }

        if (user.MemberId.HasValue)
        {
            claims.Add(new("member_id", user.MemberId.Value.ToString()));
        }

        // Compute accessible org IDs from all UserOrganizationRoles for this user
        var roles = await db.UserOrganizationRoles
            .Where(r => r.UserId == user.Id && r.IsActive)
            .ToListAsync();

        bool isSystemAdmin = await userManager.IsInRoleAsync(user, "SystemAdmin");
        if (isSystemAdmin)
        {
            claims.Add(new("system_admin", "true"));
            claims.Add(new(ClaimTypes.Role, "SystemAdmin"));
        }
        else
        {
            HashSet<int> accessibleOrgIds = new HashSet<int>();
            foreach (var role in roles)
            {
                var subtree = await hierarchyService.GetDescendantOrgIdsAsync(role.OrganizationId);
                foreach (int id in subtree)
                {
                    accessibleOrgIds.Add(id);
                }
            }
            foreach (int orgId in accessibleOrgIds)
            {
                claims.Add(new("accessible_org", orgId.ToString()));
            }
        }

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiryHours"] ?? "24")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string FirstName, string LastName, string Email, string Password);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record SetupAccountRequest(string Email, string Token, string NewPassword);
