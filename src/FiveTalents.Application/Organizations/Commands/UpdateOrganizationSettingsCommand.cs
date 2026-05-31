using FiveTalents.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Organizations.Commands;

public record UpdateOrganizationSettingsCommand : IRequest
{
    public int OrganizationId { get; init; }
    public string? PrimaryColor { get; init; }
    public string? SecondaryColor { get; init; }
    public string Currency { get; init; } = "USD";
    public string FiscalYearStart { get; init; } = "01-01";
    public bool EnableOnlineGiving { get; init; }
    public bool EnableMemberPortal { get; init; }
    public bool EnableAttendanceTracking { get; init; }
    public string? GoogleWorkspaceDomain { get; init; }
    public bool GoogleWorkspaceEnabled { get; init; }
}

public class UpdateOrganizationSettingsCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateOrganizationSettingsCommand>
{
    public async Task Handle(UpdateOrganizationSettingsCommand request, CancellationToken cancellationToken)
    {
        var settings = await db.OrganizationSettings
            .FirstOrDefaultAsync(s => s.OrganizationId == request.OrganizationId, cancellationToken);

        if (settings is null)
        {
            settings = new Domain.Organizations.OrganizationSettings { OrganizationId = request.OrganizationId };
            db.OrganizationSettings.Add(settings);
        }

        settings.PrimaryColor = request.PrimaryColor;
        settings.SecondaryColor = request.SecondaryColor;
        settings.Currency = request.Currency;
        settings.FiscalYearStart = request.FiscalYearStart;
        settings.EnableOnlineGiving = request.EnableOnlineGiving;
        settings.EnableMemberPortal = request.EnableMemberPortal;
        settings.EnableAttendanceTracking = request.EnableAttendanceTracking;
        settings.GoogleWorkspaceDomain = request.GoogleWorkspaceDomain;
        settings.GoogleWorkspaceEnabled = request.GoogleWorkspaceEnabled;

        await db.SaveChangesAsync(cancellationToken);
    }
}
