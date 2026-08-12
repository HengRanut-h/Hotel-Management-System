using HotelManagement.Domain.Common.Exceptions;

namespace HotelManagement.Domain.Common.Guards;

public static class Guard
{
    public static string Required(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }

    public static decimal NonNegative(decimal value, string name)
    {
        if (value < 0) throw new DomainException($"{name} cannot be negative.");
        return value;
    }
}