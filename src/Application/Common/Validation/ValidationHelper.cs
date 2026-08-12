using HotelManagement.Application.Common.Exceptions;

namespace HotelManagement.Application.Common.Validation;

public static class ValidationHelper
{
    public static void Required(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ConflictException($"{name} is required.");
    }

    public static void Page(ref int page, ref int size)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 100);
    }
}