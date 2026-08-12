namespace HotelManagement.Application.Abstractions.Payments;

public interface IPaymentGateway
{
    Task<string> CreateReferenceAsync(decimal amount, string currency, CancellationToken ct = default);
}