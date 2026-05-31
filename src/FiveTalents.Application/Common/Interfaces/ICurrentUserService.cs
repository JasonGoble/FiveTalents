namespace FiveTalents.Application.Common.Interfaces;

public interface ICurrentUserService
{
    public string? UserId { get; }
    public string? UserName { get; }
    public int? OrganizationId { get; }
    public bool IsAuthenticated { get; }
    public bool IsInRole(string role);
    public IEnumerable<int> GetAccessibleOrganizationIds();
}
