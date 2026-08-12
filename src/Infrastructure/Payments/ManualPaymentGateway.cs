using HotelManagement.Application.Abstractions.Payments;

namespace HotelManagement.Infrastructure.Payments;

public sealed class ManualPaymentGateway : IPaymentGateway
{
    public Task<string> CreateReferenceAsync(
        decimal amount,
        string currency,
        CancellationToken ct = default)
    {
        var reference =
            $"PAY-{System.DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

        return Task.FromResult(reference);
    }
}