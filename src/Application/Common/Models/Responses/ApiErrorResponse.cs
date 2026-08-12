namespace HotelManagement.Application.Common.Models.Responses;

public sealed record ApiErrorResponse(
    bool Success,
    int Status,
    string Code,
    string Message,
    object? Errors,
    DateTimeOffset TimestampUtc,
    string? TraceId)
{
    public static ApiErrorResponse Create(
        int status,
        string code,
        string message,
        object? errors = null,
        string? traceId = null) =>
        new(
            false,
            status,
            code,
            message,
            errors,
            DateTimeOffset.UtcNow,
            traceId);
}