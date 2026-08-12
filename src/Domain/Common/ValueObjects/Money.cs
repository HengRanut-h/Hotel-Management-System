namespace HotelManagement.Domain.Common.ValueObjects;

public readonly record struct Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency = "USD") => new(0m, currency.ToUpperInvariant());
}