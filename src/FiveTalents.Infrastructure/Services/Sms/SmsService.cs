using FiveTalents.Application.Common.Interfaces;

using Microsoft.Extensions.Logging;

namespace FiveTalents.Infrastructure.Services.Sms;

public class SmsService(ILogger<SmsService> logger) : ISmsService
{
    public async Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        // TODO: implement Twilio / AWS SNS
        logger.LogInformation("SMS to {PhoneNumber}: {Message}", phoneNumber, message);
        await Task.CompletedTask;
    }
}
