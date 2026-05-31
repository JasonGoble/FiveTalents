using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Organizations.DTOs;
using FiveTalents.Domain.Organizations;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Organizations.Queries;

public record GetOrganizationTreeQuery : IRequest<IEnumerable<OrganizationTreeDto>>;

public class GetOrganizationTreeQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetOrganizationTreeQuery, IEnumerable<OrganizationTreeDto>>
{
    public async Task<IEnumerable<OrganizationTreeDto>> Handle(
        GetOrganizationTreeQuery request, CancellationToken cancellationToken)
    {
        var orgs = await db.Organizations
            .Where(o => !o.IsDeleted && o.IsActive)
            .OrderBy(o => o.Level)
            .ThenBy(o => o.Name)
            .ToListAsync(cancellationToken);

        return BuildTree(orgs, null);
    }

    private static IEnumerable<OrganizationTreeDto> BuildTree(
        IReadOnlyList<Organization> all, int? parentId)
        => all
            .Where(o => o.ParentOrganizationId == parentId)
            .Select(o => new OrganizationTreeDto(
                o.Id, o.Name, o.Level, o.IsActive, BuildTree(all, o.Id)));
}
