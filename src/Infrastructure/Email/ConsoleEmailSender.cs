using HotelManagement.Application.Abstractions.Email;

namespace HotelManagement.Infrastructure.Email;

public sealed class ConsoleEmailSender(ILogger<ConsoleEmailSender> log) : IEmailSender
{
    public Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        log.LogInformation("EMAIL to {To}: {Subject}", to, subject);
        return Task.CompletedTask;
    }
}