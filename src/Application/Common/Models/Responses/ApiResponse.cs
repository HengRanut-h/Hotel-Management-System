namespace HotelManagement.Application.Common.Models.Responses;

public sealed record ApiResponse<T>(
    bool Success,
    int Status,
    string Code,
    string Message,
    T? Data,
    object? Meta,
    DateTimeOffset TimestampUtc,
    string? TraceId)
{
    public static ApiResponse<T> Ok(
        T data,
        string message = "Success",
        string code = "SUCCESS",
        object? meta = null,
        string? traceId = null) =>
        new(
            true,
            200,
            code,
            message,
            data,
            meta,
            DateTimeOffset.UtcNow,
            traceId);

    public static ApiResponse<T> Created(
        T data,
        string message = "Created successfully",
        string code = "CREATED",
        string? traceId = null) =>
        new(
            true,
            201,
            code,
            message,
            data,
            null,
            DateTimeOffset.UtcNow,
            traceId);

    public static ApiResponse<T> Updated(
        T data,
        string message = "Updated successfully",
        string? traceId = null) =>
        new(
            true,
            200,
            "UPDATED",
            message,
            data,
            null,
            DateTimeOffset.UtcNow,
            traceId);

    public static ApiResponse<T> Activated(
        T data,
        string message = "Activated successfully",
        string? traceId = null) =>
        new(
            true,
            200,
            "ACTIVATED",
            message,
            data,
            null,
            DateTimeOffset.UtcNow,
            traceId);

    public static ApiResponse<T> Deactivated(
        T data,
        string message = "Deactivated successfully",
        string? traceId = null) =>
        new(
            true,
            200,
            "DEACTIVATED",
            message,
            data,
            null,
            DateTimeOffset.UtcNow,
            traceId);

    public static ApiResponse<T> Restored(
        T data,
        string message = "Restored successfully",
        string? traceId = null) =>
        new(
            true,
            200,
            "RESTORED",
            message,
            data,
            null,
            DateTimeOffset.UtcNow,
            traceId);

    public static ApiResponse<T> Action(
        T data,
        string message,
        string code = "ACTION_COMPLETED",
        string? traceId = null) =>
        new(
            true,
            200,
            code,
            message,
            data,
            null,
            DateTimeOffset.UtcNow,
            traceId);
}