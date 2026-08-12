using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Application.Features.Permissions.Contracts;

// =========================================================
// QUERY
// Search + Filter + Sort + Pagination
// =========================================================

public sealed class PermissionQuery
{
    // General search:
    // - Name
    // - Id when search is a valid GUID
    public string? Search { get; init; }

    // Field-specific search
    public string? Name { get; init; }

    public Guid? CreatedBy { get; init; }

    public DateOnly? CreatedFrom { get; init; }

    public DateOnly? CreatedTo { get; init; }

    // name
    // createdAt
    // updatedAt
    // createdBy
    // roleCount
    public string SortBy { get; init; } = "name";

    // asc | desc
    public string SortDirection { get; init; } = "asc";

    [Range(1, int.MaxValue)]
    public int PageNumber { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;
}

// =========================================================
// CREATE
// =========================================================

public sealed class CreatePermissionRequest
{
    [Required]
    [StringLength(
        150,
        MinimumLength = 3)]
    [RegularExpression(
        "^[a-zA-Z0-9]+(?:[.-][a-zA-Z0-9]+)*$",
        ErrorMessage =
            "Permission name may contain letters, numbers, dots, and hyphens only.")]
    public string Name { get; init; } = string.Empty;
}

// =========================================================
// UPDATE
// =========================================================

public sealed class UpdatePermissionRequest
{
    [Required]
    [StringLength(
        150,
        MinimumLength = 3)]
    [RegularExpression(
        "^[a-zA-Z0-9]+(?:[.-][a-zA-Z0-9]+)*$",
        ErrorMessage =
            "Permission name may contain letters, numbers, dots, and hyphens only.")]
    public string Name { get; init; } = string.Empty;
}

// =========================================================
// RESPONSE
// =========================================================

public sealed record PermissionResponse(
    Guid Id,
    string Name,
    DateTimeOffset CreatedAtUtc,
    Guid? CreatedBy,
    DateTimeOffset? UpdatedAtUtc,
    Guid? UpdatedBy,
    int RoleCount);