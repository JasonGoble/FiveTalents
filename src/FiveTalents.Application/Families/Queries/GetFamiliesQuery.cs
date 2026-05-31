using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Families.DTOs;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Families.Queries;

public record GetFamiliesQuery(int OrganizationId) : IRequest<IReadOnlyList<FamilySummaryDto>>;

public class GetFamiliesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetFamiliesQuery, IReadOnlyList<FamilySummaryDto>>
{
    public async Task<IReadOnlyList<FamilySummaryDto>> Handle(
        GetFamiliesQuery request, CancellationToken cancellationToken)
    {
        return await db.Families
            .Where(f => f.OrganizationId == request.OrganizationId && !f.IsDeleted)
            .OrderBy(f => f.Name)
            .Select(f => new FamilySummaryDto(
                f.Id,
                f.Name,
                f.Members.Count,
                f.OrganizationId))
            .ToListAsync(cancellationToken);
    }
}
