namespace HotelManagement.Application.Common.Models.Responses;

public sealed record CatalogResponse(
    Guid Id,
    Guid HotelId,
    Guid? BranchId,
    string Name,
    string Code,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAtUtc);