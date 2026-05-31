namespace FiveTalents.Application.Common.Interfaces;

public interface ISmsService
{
    public Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}
