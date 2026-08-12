namespace HotelManagement.Application.Common.Models.Requests;

public sealed record OperationalCreateRequest(
    string Title,
    string? Notes = null,
    decimal Amount = 0,
    DateTimeOffset? EventAtUtc = null,
    Guid? BranchId = null,
    Guid? RelatedEntityId = null,
    string? RelatedEntityType = null,
    string Status = "Open");