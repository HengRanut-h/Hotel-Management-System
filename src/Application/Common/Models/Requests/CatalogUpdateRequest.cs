namespace HotelManagement.Application.Common.Models.Requests;

public sealed record CatalogUpdateRequest(
    string Name,
    string Code,
    string? Description = null,
    bool IsActive = true);