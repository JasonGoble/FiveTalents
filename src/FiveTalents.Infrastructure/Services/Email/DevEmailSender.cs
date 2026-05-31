using FiveTalents.Application.Common.Interfaces;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FiveTalents.Infrastructure.Services.Email;

public class DevEmailSender(IOptions<SmtpSettings> options, ILogger<DevEmailSender> logger) : IEmailService
{
    private readonly SmtpSettings _settings = options.Value;

    public Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
        => SendAsync([to], subject, body, isHtml, cancellationToken);

    public async Task SendAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        List<string> recipientList = recipients.ToList();
        string dir = Path.Combine(Directory.GetCurrentDirectory(), "logs", "emails");
        Directory.CreateDirectory(dir);

        string filename = $"{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}_{Sanitize(subject)}.eml";
        string path = Path.Combine(dir, filename);

        string contentType = isHtml ? "text/html" : "text/plain";
        string date = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss +0000");
        string eml =
            $"From: {_settings.FromName} <{_settings.FromAddress}>\r\n" +
            $"To: {string.Join(", ", recipientList)}\r\n" +
            $"Subject: {subject}\r\n" +
            $"Date: {date}\r\n" +
            $"MIME-Version: 1.0\r\n" +
            $"Content-Type: {contentType}; charset=utf-8\r\n" +
            $"\r\n" +
            body;

        await File.WriteAllTextAsync(path, eml, cancellationToken);
        logger.LogInformation("Dev email written to {Path} (to: {Recipients}, subject: {Subject})", path, string.Join(", ", recipientList), subject);
    }

    private static string Sanitize(string value)
    {
        char[] invalid = Path.GetInvalidFileNameChars();
        string safe = string.Concat(value.Select(c => invalid.Contains(c) ? '_' : c));
        return safe.Length > 60 ? safe[..60] : safe;
    }
}
