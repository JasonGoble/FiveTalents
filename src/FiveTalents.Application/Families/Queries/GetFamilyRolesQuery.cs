using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Families.DTOs;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Families.Queries;

public record GetFamilyRolesQuery(int OrganizationId) : IRequest<IReadOnlyList<FamilyRoleDto>>;

public class GetFamilyRolesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetFamilyRolesQuery, IReadOnlyList<FamilyRoleDto>>
{
    public async Task<IReadOnlyList<FamilyRoleDto>> Handle(
        GetFamilyRolesQuery request, CancellationToken cancellationToken)
    {
        return await db.FamilyRoles
            .Where(r => r.OrganizationId == request.OrganizationId && r.IsActive)
            .OrderBy(r => r.SortOrder)
            .Select(r => new FamilyRoleDto(r.Id, r.Name, r.IsAdult, r.SortOrder))
            .ToListAsync(cancellationToken);
    }
}
