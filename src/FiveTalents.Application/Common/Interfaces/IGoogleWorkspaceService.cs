namespace FiveTalents.Application.Common.Interfaces;

public interface IGoogleWorkspaceService
{
    public Task<IEnumerable<GoogleWorkspaceUser>> GetUsersAsync(int organizationId, CancellationToken cancellationToken = default);
    public Task<GoogleWorkspaceUser?> GetUserAsync(string email, CancellationToken cancellationToken = default);
    public Task<bool> CreateCalendarEventAsync(int organizationId, GoogleCalendarEvent calendarEvent, CancellationToken cancellationToken = default);
    public Task<bool> SendEmailAsync(int organizationId, string to, string subject, string body, CancellationToken cancellationToken = default);
    public Task<bool> IsConfiguredForOrganizationAsync(int organizationId, CancellationToken cancellationToken = default);
}

public record GoogleWorkspaceUser(string Email, string Name, string? PhotoUrl, bool IsActive);
public record GoogleCalendarEvent(string Title, string Description, DateTime Start, DateTime End, string? Location, IEnumerable<string> AttendeeEmails);
