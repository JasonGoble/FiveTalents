using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Organizations.DTOs;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Organizations.Queries;

public record GetOrganizationByIdQuery(int Id) : IRequest<OrganizationDto?>;

public class GetOrganizationByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetOrganizationByIdQuery, OrganizationDto?>
{
    public async Task<OrganizationDto?> Handle(
        GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        var o = await db.Organizations
            .Include(o => o.ParentOrganization)
            .FirstOrDefaultAsync(o => o.Id == request.Id && !o.IsDeleted, cancellationToken);

        if (o is null)
        {
            return null;
        }

        int memberCount = await db.Members
            .CountAsync(m => m.OrganizationId == o.Id && !m.IsDeleted, cancellationToken);

        return new OrganizationDto(
            o.Id, o.Name, o.Level, o.ParentOrganizationId, o.ParentOrganization?.Name,
            o.IsActive, memberCount,
            o.Description, o.Email, o.PhoneNumber, o.Website, o.City, o.State, o.Country, o.TimeZone);
    }
}
