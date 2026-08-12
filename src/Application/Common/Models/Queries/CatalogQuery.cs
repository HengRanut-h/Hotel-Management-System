namespace HotelManagement.Application.Common.Models.Queries;

public sealed record CatalogQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    bool? IsActive = null,
    string SortBy = "name",
    string SortDirection = "asc");