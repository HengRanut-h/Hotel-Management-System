using HotelManagement.Application.Abstractions.DateTime;

namespace HotelManagement.Infrastructure.DateTime;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}