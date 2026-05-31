namespace FiveTalents.Application.Common.Interfaces;

public interface IEmailService
{
    public Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
    public Task SendAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
}
