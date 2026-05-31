using FiveTalents.Application.Common.Interfaces;

using Microsoft.Extensions.Logging;

namespace FiveTalents.Infrastructure.Services.GoogleWorkspace;

public class GoogleWorkspaceService(ILogger<GoogleWorkspaceService> logger) : IGoogleWorkspaceService
{
    public async Task<IEnumerable<GoogleWorkspaceUser>> GetUsersAsync(int organizationId, CancellationToken cancellationToken = default)
    {
        // TODO: implement Google Admin SDK
        logger.LogInformation("Getting Google Workspace users for org {OrganizationId}", organizationId);
        return await Task.FromResult(Enumerable.Empty<GoogleWorkspaceUser>());
    }

    public async Task<GoogleWorkspaceUser?> GetUserAsync(string email, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting Google Workspace user {Email}", email);
        return await Task.FromResult<GoogleWorkspaceUser?>(null);
    }

    public async Task<bool> CreateCalendarEventAsync(int organizationId, GoogleCalendarEvent calendarEvent, CancellationToken cancellationToken = default)
    {
        // TODO: implement Google Calendar API
        logger.LogInformation("Creating Google Calendar event for org {OrganizationId}: {Title}", organizationId, calendarEvent.Title);
        return await Task.FromResult(false);
    }

    public async Task<bool> SendEmailAsync(int organizationId, string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        // TODO: implement Gmail API
        logger.LogInformation("Sending Gmail for org {OrganizationId} to {To}", organizationId, to);
        return await Task.FromResult(false);
    }

    public async Task<bool> IsConfiguredForOrganizationAsync(int organizationId, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(false);
    }
}
