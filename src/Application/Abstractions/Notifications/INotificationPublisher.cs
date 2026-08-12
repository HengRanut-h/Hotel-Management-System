namespace HotelManagement.Application.Abstractions.Notifications;

public interface INotificationPublisher
{
    Task PublishAsync(Guid hotelId, Guid? userId, string title, string message, CancellationToken ct = default);
}