using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Application.Features.Permissions.Contracts;
using HotelManagement.Domain.Modules.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Permissions;

public sealed class PermissionsService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public PermissionsService(
        IApplicationDbContext db,
        ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    // =========================================================
    // GET ALL
    //
    // CRUD +
    // SEARCH +
    // FIELD SEARCH +
    // FILTER +
    // SORT +
    // PAGINATION
    // =========================================================

    public async Task<PagedResult<PermissionResponse>> GetAllAsync(
        PermissionQuery request,
        CancellationToken cancellationToken = default)
    {
        // =====================================================
        // PAGINATION
        // =====================================================

        var pageNumber =
            Math.Max(
                1,
                request.PageNumber);

        var pageSize =
            Math.Clamp(
                request.PageSize,
                1,
                100);

        // =====================================================
        // BASE QUERY
        //
        // PermissionConfiguration already has:
        // HasQueryFilter(x => !x.IsDeleted)
        // =====================================================

        IQueryable<Permission> query =
            _db.Permissions
                .AsNoTracking();

        // =====================================================
        // GENERAL SEARCH
        //
        // Search Name
        // Search Id if term is valid GUID
        // =====================================================

        if (!string.IsNullOrWhiteSpace(
                request.Search))
        {
            var term =
                request.Search.Trim();

            if (Guid.TryParse(
                    term,
                    out var permissionId))
            {
                query =
                    query.Where(
                        x =>
                            x.Id == permissionId ||
                            x.Name.Contains(term));
            }
            else
            {
                query =
                    query.Where(
                        x =>
                            x.Name.Contains(term));
            }
        }

        // =====================================================
        // NAME FIELD SEARCH
        // =====================================================

        if (!string.IsNullOrWhiteSpace(
                request.Name))
        {
            var name =
                request.Name.Trim();

            query =
                query.Where(
                    x =>
                        x.Name.Contains(name));
        }

        // =====================================================
        // CREATED BY FILTER
        // =====================================================

        if (request.CreatedBy.HasValue)
        {
            query =
                query.Where(
                    x =>
                        x.CreatedBy ==
                        request.CreatedBy.Value);
        }

        // =====================================================
        // CREATED FROM
        // =====================================================

        if (request.CreatedFrom.HasValue)
        {
            var from =
                new DateTimeOffset(
                    request.CreatedFrom.Value
                        .ToDateTime(
                            TimeOnly.MinValue),
                    TimeSpan.Zero);

            query =
                query.Where(
                    x =>
                        x.CreatedAtUtc >= from);
        }

        // =====================================================
        // CREATED TO
        //
        // Exclusive next-day comparison allows:
        //
        // createdTo=2026-08-11
        //
        // to include the whole August 11.
        // =====================================================

        if (request.CreatedTo.HasValue)
        {
            var toExclusive =
                new DateTimeOffset(
                    request.CreatedTo.Value
                        .AddDays(1)
                        .ToDateTime(
                            TimeOnly.MinValue),
                    TimeSpan.Zero);

            query =
                query.Where(
                    x =>
                        x.CreatedAtUtc <
                        toExclusive);
        }

        // =====================================================
        // SORTING
        // =====================================================

        query =
            ApplySorting(
                query,
                request.SortBy,
                request.SortDirection);

        // =====================================================
        // TOTAL ITEMS
        // =====================================================

        var totalItems =
            await query.CountAsync(
                cancellationToken);

        // =====================================================
        // PAGINATION + PROJECTION
        // =====================================================

        var items =
            await query
                .Skip(
                    (pageNumber - 1) *
                    pageSize)
                .Take(pageSize)
                .Select(
                    x =>
                        new PermissionResponse(
                            x.Id,
                            x.Name,
                            x.CreatedAtUtc,
                            x.CreatedBy,
                            x.UpdatedAtUtc,
                            x.UpdatedBy,
                            x.RolePermissions.Count))
                .ToListAsync(
                    cancellationToken);

        // =====================================================
        // RESPONSE
        // =====================================================

        return new PagedResult<PermissionResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<PermissionResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var permission =
            await _db.Permissions
                .AsNoTracking()
                .Where(
                    x =>
                        x.Id == id)
                .Select(
                    x =>
                        new PermissionResponse(
                            x.Id,
                            x.Name,
                            x.CreatedAtUtc,
                            x.CreatedBy,
                            x.UpdatedAtUtc,
                            x.UpdatedBy,
                            x.RolePermissions.Count))
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (permission is null)
        {
            throw new NotFoundException(
                "Permission not found.");
        }

        return permission;
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<PermissionResponse> CreateAsync(
        CreatePermissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var name =
            NormalizeName(
                request.Name);

        // =====================================================
        // CHECK DUPLICATE
        //
        // IgnoreQueryFilters is intentional.
        //
        // Even a soft-deleted row still exists in MySQL and
        // Name has a UNIQUE index.
        // =====================================================

        var exists =
            await _db.Permissions
                .IgnoreQueryFilters()
                .AnyAsync(
                    x =>
                        x.Name == name,
                    cancellationToken);

        if (exists)
        {
            throw new ConflictException(
                $"Permission '{name}' already exists.");
        }

        // =====================================================
        // CREATE ENTITY
        // =====================================================

        var permission =
            new Permission(name);

        _db.Permissions.Add(
            permission);

        await _db.SaveChangesAsync(
            cancellationToken);

        return await GetByIdAsync(
            permission.Id,
            cancellationToken);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<PermissionResponse> UpdateAsync(
        Guid id,
        UpdatePermissionRequest request,
        CancellationToken cancellationToken = default)
    {
        // =====================================================
        // FIND
        // =====================================================

        var permission =
            await _db.Permissions
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == id,
                    cancellationToken);

        if (permission is null)
        {
            throw new NotFoundException(
                "Permission not found.");
        }

        // =====================================================
        // NORMALIZE
        // =====================================================

        var name =
            NormalizeName(
                request.Name);

        // =====================================================
        // DUPLICATE CHECK
        // =====================================================

        var duplicate =
            await _db.Permissions
                .IgnoreQueryFilters()
                .AnyAsync(
                    x =>
                        x.Id != id &&
                        x.Name == name,
                    cancellationToken);

        if (duplicate)
        {
            throw new ConflictException(
                $"Permission '{name}' already exists.");
        }

        // =====================================================
        // UPDATE ENTITY
        // =====================================================

        permission.UpdateName(
            name);

        await _db.SaveChangesAsync(
            cancellationToken);

        return await GetByIdAsync(
            id,
            cancellationToken);
    }

    // =========================================================
    // DELETE
    // SOFT DELETE
    // =========================================================

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        // =====================================================
        // LOAD ROLE RELATIONSHIPS
        // =====================================================

        var permission =
            await _db.Permissions
                .Include(
                    x =>
                        x.RolePermissions)
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == id,
                    cancellationToken);

        if (permission is null)
        {
            throw new NotFoundException(
                "Permission not found.");
        }

        // =====================================================
        // DON'T DELETE ASSIGNED PERMISSION
        // =====================================================

        if (permission.RolePermissions.Count > 0)
        {
            throw new ConflictException(
                "Cannot delete a permission that is assigned " +
                "to one or more roles. Remove it from the roles first.");
        }

        // =====================================================
        // SOFT DELETE
        // =====================================================

        permission.SoftDelete(
            _currentUser.UserId);

        await _db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // SORT
    // =========================================================

    private static IQueryable<Permission> ApplySorting(
        IQueryable<Permission> query,
        string? sortBy,
        string? sortDirection)
    {
        var descending =
            string.Equals(
                sortDirection,
                "desc",
                StringComparison.OrdinalIgnoreCase);

        var field =
            sortBy?
                .Trim()
                .ToLowerInvariant();

        return field switch
        {
            // =================================================
            // CREATED AT
            // =================================================

            "createdat" or
            "createdatutc" =>
                descending
                    ? query.OrderByDescending(
                        x => x.CreatedAtUtc)
                    : query.OrderBy(
                        x => x.CreatedAtUtc),

            // =================================================
            // UPDATED AT
            // =================================================

            "updatedat" or
            "updatedatutc" =>
                descending
                    ? query.OrderByDescending(
                        x => x.UpdatedAtUtc)
                    : query.OrderBy(
                        x => x.UpdatedAtUtc),

            // =================================================
            // CREATED BY
            // =================================================

            "createdby" =>
                descending
                    ? query.OrderByDescending(
                        x => x.CreatedBy)
                    : query.OrderBy(
                        x => x.CreatedBy),

            // =================================================
            // ROLE COUNT
            // =================================================

            "rolecount" =>
                descending
                    ? query
                        .OrderByDescending(
                            x =>
                                x.RolePermissions.Count)
                        .ThenBy(
                            x =>
                                x.Name)
                    : query
                        .OrderBy(
                            x =>
                                x.RolePermissions.Count)
                        .ThenBy(
                            x =>
                                x.Name),

            // =================================================
            // DEFAULT = NAME
            // =================================================

            _ =>
                descending
                    ? query.OrderByDescending(
                        x => x.Name)
                    : query.OrderBy(
                        x => x.Name)
        };
    }

    // =========================================================
    // NORMALIZE PERMISSION NAME
    //
    // Rooms.View
    // becomes:
    // rooms.view
    // =========================================================

    private static string NormalizeName(
        string name)
    {
        if (string.IsNullOrWhiteSpace(
                name))
        {
            throw new ArgumentException(
                "Permission name is required.",
                nameof(name));
        }

        return name
            .Trim()
            .ToLowerInvariant();
    }
}