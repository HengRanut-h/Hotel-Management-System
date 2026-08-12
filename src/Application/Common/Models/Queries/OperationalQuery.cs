namespace HotelManagement.Application.Common.Models.Queries;

public sealed record OperationalQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    string? Status = null,
    string SortBy = "createdAt",
    string SortDirection = "desc");