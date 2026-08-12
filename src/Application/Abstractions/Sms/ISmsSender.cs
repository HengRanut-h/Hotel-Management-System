namespace HotelManagement.Application.Abstractions.Sms;

public interface ISmsSender
{
    Task SendAsync(string to, string message, CancellationToken ct = default);
}