namespace HotelManagement.Application.Common.Models.Responses;

public sealed record OperationalResponse(
    Guid Id,
    Guid HotelId,
    Guid? BranchId,
    string ReferenceNumber,
    string Title,
    string Status,
    string? Notes,
    decimal Amount,
    DateTimeOffset EventAtUtc,
    Guid? RelatedEntityId,
    string? RelatedEntityType,
    DateTimeOffset CreatedAtUtc);