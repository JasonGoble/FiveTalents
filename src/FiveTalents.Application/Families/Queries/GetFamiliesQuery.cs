using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Families.DTOs;
using FiveTalents.Domain.Auth;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Families.Queries;

public record GetFamiliesQuery(int? OrganizationId) : IRequest<IReadOnlyList<FamilySummaryDto>>;

public class GetFamiliesQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetFamiliesQuery, IReadOnlyList<FamilySummaryDto>>
{
    public async Task<IReadOnlyList<FamilySummaryDto>> Handle(
        GetFamiliesQuery request, CancellationToken cancellationToken)
    {
        bool isSystemAdmin = currentUser.IsInRole(AppRoles.SystemAdmin);

        if (request.OrganizationId is null && !isSystemAdmin)
        {
            throw new ForbiddenAccessException();
        }

        Dictionary<int, string> orgNames = [];
        if (isSystemAdmin)
        {
            orgNames = await db.Organizations
                .ToDictionaryAsync(o => o.Id, o => o.Name, cancellationToken);
        }

        IQueryable<Domain.Families.Family> query = request.OrganizationId is null
            ? db.Families.Where(f => !f.IsDeleted)
            : db.Families.Where(f => f.OrganizationId == request.OrganizationId && !f.IsDeleted);

        var raw = await query
            .OrderBy(f => f.Name)
            .Select(f => new { f.Id, f.Name, MemberCount = f.Members.Count, f.OrganizationId })
            .ToListAsync(cancellationToken);

        return raw.Select(r => new FamilySummaryDto(
            r.Id,
            r.Name,
            r.MemberCount,
            r.OrganizationId,
            isSystemAdmin ? orgNames.GetValueOrDefault(r.OrganizationId) : null))
            .ToList();
    }
}
