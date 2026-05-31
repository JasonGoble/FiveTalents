namespace FiveTalents.Application.Common.Interfaces;

public interface IOrganizationHierarchyService
{
    public Task<IReadOnlyList<int>> GetDescendantOrgIdsAsync(int rootOrgId, CancellationToken ct = default);
}
