using System.Net;
using System.Net.Mail;

using FiveTalents.Application.Common.Interfaces;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FiveTalents.Infrastructure.Services.Email;

public class SmtpEmailService(IOptions<SmtpSettings> options, ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly SmtpSettings _settings = options.Value;

    public Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
        => SendAsync([to], subject, body, isHtml, cancellationToken);

    public async Task SendAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        if (!_settings.IsConfigured)
        {
            logger.LogWarning("SMTP not configured — skipping email to {Recipients}: {Subject}", string.Join(", ", recipients), subject);
            return;
        }

        using SmtpClient client = new SmtpClient(_settings.Host, _settings.Port);
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.EnableSsl = false;

        if (!string.IsNullOrWhiteSpace(_settings.Username))
        {
            client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
        }

        MailAddress from = new MailAddress(_settings.FromAddress, _settings.FromName);
        using MailMessage message = new MailMessage { From = from, Subject = subject, Body = body, IsBodyHtml = isHtml };
        foreach (string recipient in recipients)
        {
            message.To.Add(recipient);
        }

        await client.SendMailAsync(message, cancellationToken);
        logger.LogInformation("Email sent to {Recipients}: {Subject}", string.Join(", ", recipients), subject);
    }
}
