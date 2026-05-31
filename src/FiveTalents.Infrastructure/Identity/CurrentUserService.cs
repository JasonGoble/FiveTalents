using System.Security.Claims;

using FiveTalents.Application.Common.Interfaces;

using Microsoft.AspNetCore.Http;

namespace FiveTalents.Infrastructure.Identity;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => User?.FindFirstValue(ClaimTypes.Name);
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public int? OrganizationId
    {
        get
        {
            string? claim = User?.FindFirstValue("organization_id");
            return claim != null ? int.Parse(claim) : null;
        }
    }

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;

    public IEnumerable<int> GetAccessibleOrganizationIds()
    {
        return User?.FindAll("accessible_org")
            .Select(c => int.Parse(c.Value))
            .ToList() ?? [];
    }
}
