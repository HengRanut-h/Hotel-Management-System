namespace HotelManagement.Worker.Services;

public sealed class BackgroundJobState
{
    public DateTimeOffset StartedAtUtc { get; } = DateTimeOffset.UtcNow;
}