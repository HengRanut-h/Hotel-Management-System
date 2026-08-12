namespace HotelManagement.Application.Common.Models.Requests;

public sealed record CatalogCreateRequest(
    string Name,
    string Code,
    string? Description = null,
    Guid? BranchId = null);