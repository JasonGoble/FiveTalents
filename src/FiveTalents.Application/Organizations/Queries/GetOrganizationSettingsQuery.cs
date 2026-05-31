using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Organizations.DTOs;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Organizations.Queries;

public record GetOrganizationSettingsQuery(int OrganizationId) : IRequest<OrganizationSettingsDto>;

public class GetOrganizationSettingsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetOrganizationSettingsQuery, OrganizationSettingsDto>
{
    public async Task<OrganizationSettingsDto> Handle(
        GetOrganizationSettingsQuery request, CancellationToken cancellationToken)
    {
        var s = await db.OrganizationSettings
            .FirstOrDefaultAsync(s => s.OrganizationId == request.OrganizationId, cancellationToken);

        return new OrganizationSettingsDto(
            request.OrganizationId,
            s?.PrimaryColor, s?.SecondaryColor,
            s?.Currency ?? "USD", s?.FiscalYearStart ?? "01-01",
            s?.EnableOnlineGiving ?? false, s?.EnableMemberPortal ?? false,
            s?.EnableAttendanceTracking ?? true,
            s?.GoogleWorkspaceDomain, s?.GoogleWorkspaceEnabled ?? false);
    }
}
