namespace HotelManagement.Application.Common.Models.Requests;

public sealed record OperationalUpdateRequest(
    string Title,
    string? Notes,
    decimal Amount,
    DateTimeOffset EventAtUtc,
    Guid? RelatedEntityId = null,
    string? RelatedEntityType = null);