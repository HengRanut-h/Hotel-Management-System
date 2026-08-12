using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Common.Services;

public sealed class CatalogCrudService<TEntity>(IApplicationDbContext db, ICurrentUser currentUser)
    where TEntity : class, ICatalogEntity
{
    private Guid HotelId => currentUser.HotelId ?? throw new ForbiddenException("A hotel context is required.");

    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<PagedResult<CatalogResponse>> GetAllAsync(
        CatalogQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var hotelId = HotelId;

        var query = db.Set<TEntity>()
            .AsNoTracking()
            .Where(entity =>
                EF.Property<Guid>(entity, "HotelId") == hotelId &&
                !EF.Property<bool>(entity, "IsDeleted"));

        // =====================================================
        // SEARCH
        // =====================================================

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim();

            query = query.Where(entity =>
                EF.Property<string>(entity, "Name").Contains(searchTerm) ||
                EF.Property<string>(entity, "Code").Contains(searchTerm));
        }

        // =====================================================
        // ACTIVE / INACTIVE FILTER
        // =====================================================

        if (request.IsActive.HasValue)
        {
            query = query.Where(entity =>
                EF.Property<bool>(entity, "IsActive") == request.IsActive.Value);
        }

        // =====================================================
        // SORT
        // =====================================================

        query = request.SortBy.ToLowerInvariant() switch
        {
            "code" => request.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(entity => EF.Property<string>(entity, "Code"))
                : query.OrderBy(entity => EF.Property<string>(entity, "Code")),

            "createdat" => request.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderBy(entity => EF.Property<DateTimeOffset>(entity, "CreatedAtUtc"))
                : query.OrderByDescending(entity => EF.Property<DateTimeOffset>(entity, "CreatedAtUtc")),

            "active" => request.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(entity => EF.Property<bool>(entity, "IsActive"))
                : query.OrderBy(entity => EF.Property<bool>(entity, "IsActive")),

            _ => request.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(entity => EF.Property<string>(entity, "Name"))
                : query.OrderBy(entity => EF.Property<string>(entity, "Name"))
        };

        // =====================================================
        // PAGINATION
        // =====================================================

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CatalogResponse>
        {
            Items = items.Select(ToResponse).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<CatalogResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var entity = await db.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                entity =>
                    EF.Property<Guid>(entity, "HotelId") == hotelId &&
                    EF.Property<Guid>(entity, "Id") == id &&
                    !EF.Property<bool>(entity, "IsDeleted"),
                cancellationToken)
            ?? throw new NotFoundException($"{typeof(TEntity).Name} not found.");

        return ToResponse(entity);
    }

    // =========================================================
    // GET BY CODE
    // =========================================================

    public async Task<CatalogResponse> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;
        var normalizedCode = code.Trim().ToUpperInvariant();

        var entity = await db.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                entity =>
                    EF.Property<Guid>(entity, "HotelId") == hotelId &&
                    EF.Property<string>(entity, "Code") == normalizedCode &&
                    !EF.Property<bool>(entity, "IsDeleted"),
                cancellationToken)
            ?? throw new NotFoundException($"{typeof(TEntity).Name} not found.");

        return ToResponse(entity);
    }

    // =========================================================
    // GET ACTIVE
    // =========================================================

    public async Task<IReadOnlyList<CatalogResponse>> GetActiveAsync(
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var items = await db.Set<TEntity>()
            .AsNoTracking()
            .Where(entity =>
                EF.Property<Guid>(entity, "HotelId") == hotelId &&
                EF.Property<bool>(entity, "IsActive") &&
                !EF.Property<bool>(entity, "IsDeleted"))
            .OrderBy(entity => EF.Property<string>(entity, "Name"))
            .ToListAsync(cancellationToken);

        return items.Select(ToResponse).ToList();
    }

    // =========================================================
    // COUNT
    // =========================================================

    public Task<int> CountAsync(
        bool? isActive,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var query = db.Set<TEntity>()
            .AsNoTracking()
            .Where(entity =>
                EF.Property<Guid>(entity, "HotelId") == hotelId &&
                !EF.Property<bool>(entity, "IsDeleted"));

        if (isActive.HasValue)
        {
            query = query.Where(entity =>
                EF.Property<bool>(entity, "IsActive") == isActive.Value);
        }

        return query.CountAsync(cancellationToken);
    }

    // =========================================================
    // EXISTS BY CODE
    // =========================================================

    public Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;
        var normalizedCode = code.Trim().ToUpperInvariant();

        return db.Set<TEntity>()
            .AsNoTracking()
            .AnyAsync(
                entity =>
                    EF.Property<Guid>(entity, "HotelId") == hotelId &&
                    EF.Property<string>(entity, "Code") == normalizedCode &&
                    !EF.Property<bool>(entity, "IsDeleted"),
                cancellationToken);
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<CatalogResponse> CreateAsync(
        CatalogCreateRequest request,
        Func<Guid, Guid?, CatalogCreateRequest, TEntity> factory,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;
        var code = request.Code.Trim().ToUpperInvariant();

        var exists = await db.Set<TEntity>()
            .AnyAsync(
                entity =>
                    EF.Property<Guid>(entity, "HotelId") == hotelId &&
                    EF.Property<string>(entity, "Code") == code &&
                    !EF.Property<bool>(entity, "IsDeleted"),
                cancellationToken);

        if (exists)
        {
            throw new ConflictException($"{typeof(TEntity).Name} code already exists.");
        }

        var entity = factory(
            hotelId,
            request.BranchId ?? currentUser.BranchId,
            request);

        db.Set<TEntity>().Add(entity);

        await db.SaveChangesAsync(cancellationToken);

        return ToResponse(entity);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<CatalogResponse> UpdateAsync(
        Guid id,
        CatalogUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var entity = await db.Set<TEntity>()
            .FirstOrDefaultAsync(
                entity =>
                    EF.Property<Guid>(entity, "HotelId") == hotelId &&
                    EF.Property<Guid>(entity, "Id") == id &&
                    !EF.Property<bool>(entity, "IsDeleted"),
                cancellationToken)
            ?? throw new NotFoundException($"{typeof(TEntity).Name} not found.");

        var code = request.Code.Trim().ToUpperInvariant();

        var duplicateExists = await db.Set<TEntity>()
            .AnyAsync(
                item =>
                    EF.Property<Guid>(item, "HotelId") == hotelId &&
                    EF.Property<Guid>(item, "Id") != id &&
                    EF.Property<string>(item, "Code") == code &&
                    !EF.Property<bool>(item, "IsDeleted"),
                cancellationToken);

        if (duplicateExists)
        {
            throw new ConflictException($"{typeof(TEntity).Name} code already exists.");
        }

        entity.UpdateCatalog(
            request.Name,
            request.Code,
            request.Description);

        entity.SetActive(request.IsActive);

        await db.SaveChangesAsync(cancellationToken);

        return ToResponse(entity);
    }

    // =========================================================
    // ACTIVE / INACTIVE
    // =========================================================

    public async Task<CatalogResponse> SetActiveAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var entity = await db.Set<TEntity>()
            .FirstOrDefaultAsync(
                entity =>
                    EF.Property<Guid>(entity, "HotelId") == hotelId &&
                    EF.Property<Guid>(entity, "Id") == id &&
                    !EF.Property<bool>(entity, "IsDeleted"),
                cancellationToken)
            ?? throw new NotFoundException($"{typeof(TEntity).Name} not found.");

        entity.SetActive(isActive);

        await db.SaveChangesAsync(cancellationToken);

        return ToResponse(entity);
    }

    // =========================================================
    // SOFT DELETE
    // =========================================================

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var entity = await db.Set<TEntity>()
            .FirstOrDefaultAsync(
                entity =>
                    EF.Property<Guid>(entity, "HotelId") == hotelId &&
                    EF.Property<Guid>(entity, "Id") == id &&
                    !EF.Property<bool>(entity, "IsDeleted"),
                cancellationToken)
            ?? throw new NotFoundException($"{typeof(TEntity).Name} not found.");

        entity.SoftDelete(currentUser.UserId);

        await db.SaveChangesAsync(cancellationToken);
    }

    // =========================================================
    // RESTORE
    // =========================================================

    public async Task<CatalogResponse> RestoreAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var entity = await db.Set<TEntity>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                entity =>
                    EF.Property<Guid>(entity, "HotelId") == hotelId &&
                    EF.Property<Guid>(entity, "Id") == id &&
                    EF.Property<bool>(entity, "IsDeleted"),
                cancellationToken)
            ?? throw new NotFoundException($"{typeof(TEntity).Name} deleted record not found.");

        entity.Restore(currentUser.UserId);

        await db.SaveChangesAsync(cancellationToken);

        return ToResponse(entity);
    }

    // =========================================================
    // RESPONSE
    // =========================================================

    private static CatalogResponse ToResponse(TEntity entity) =>
        new(
            entity.Id,
            entity.HotelId,
            entity.BranchId,
            entity.Name,
            entity.Code,
            entity.Description,
            entity.IsActive,
            entity.CreatedAtUtc);
}