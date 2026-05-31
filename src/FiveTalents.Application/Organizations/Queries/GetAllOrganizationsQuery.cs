using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Organizations.DTOs;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Organizations.Queries;

public record GetAllOrganizationsQuery(bool IncludeInactive = false) : IRequest<IEnumerable<OrganizationDto>>;

public class GetAllOrganizationsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllOrganizationsQuery, IEnumerable<OrganizationDto>>
{
    public async Task<IEnumerable<OrganizationDto>> Handle(
        GetAllOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var query = db.Organizations
            .Include(o => o.ParentOrganization)
            .Where(o => !o.IsDeleted);

        if (!request.IncludeInactive)
        {
            query = query.Where(o => o.IsActive);
        }

        var orgs = await query
            .OrderBy(o => o.Level)
            .ThenBy(o => o.Name)
            .ToListAsync(cancellationToken);

        var memberCounts = await db.Members
            .Where(m => !m.IsDeleted)
            .GroupBy(m => m.OrganizationId)
            .Select(g => new { OrgId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.OrgId, x => x.Count, cancellationToken);

        return orgs.Select(o => new OrganizationDto(
            o.Id, o.Name, o.Level, o.ParentOrganizationId, o.ParentOrganization?.Name,
            o.IsActive, memberCounts.GetValueOrDefault(o.Id, 0),
            o.Description, o.Email, o.PhoneNumber, o.Website, o.City, o.State, o.Country, o.TimeZone));
    }
}
