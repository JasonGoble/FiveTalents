using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Groups.DTOs;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Groups.Queries;

public record GetGroupTypesQuery(int OrganizationId) : IRequest<List<GroupTypeSummaryDto>>;

public class GetGroupTypesQueryHandler(IApplicationDbContext db) : IRequestHandler<GetGroupTypesQuery, List<GroupTypeSummaryDto>>
{
    public async Task<List<GroupTypeSummaryDto>> Handle(GetGroupTypesQuery request, CancellationToken cancellationToken)
    {
        return await db.GroupTypes
            .Where(t => t.OrganizationId == request.OrganizationId && !t.IsDeleted)
            .OrderBy(t => t.Name)
            .Select(t => new GroupTypeSummaryDto(t.Id, t.Name, t.Color, t.IconName))
            .ToListAsync(cancellationToken);
    }
}
