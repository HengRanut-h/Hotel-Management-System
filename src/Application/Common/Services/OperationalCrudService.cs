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

public sealed class OperationalCrudService<TEntity>(IApplicationDbContext db, ICurrentUser currentUser)
    where TEntity : class, IOperationalRecord
{
    private Guid HotelId => currentUser.HotelId ?? throw new ForbiddenException("A hotel context is required.");

    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<PagedResult<OperationalResponse>> GetAllAsync(
        OperationalQuery request,
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
                EF.Property<string>(entity, "ReferenceNumber").Contains(searchTerm) ||
                EF.Property<string>(entity, "Title").Contains(searchTerm));
        }

        // =====================================================
        // STATUS FILTER
        // =====================================================

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(entity =>
                EF.Property<string>(entity, "Status") == request.Status);
        }

        // =====================================================
        // SORT
        // =====================================================

        query = request.SortBy.ToLowerInvariant() switch
        {
            "reference" => request.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(entity => EF.Property<string>(entity, "ReferenceNumber"))
                : query.OrderBy(entity => EF.Property<string>(entity, "ReferenceNumber")),

            "title" => request.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(entity => EF.Property<string>(entity, "Title"))
                : query.OrderBy(entity => EF.Property<string>(entity, "Title")),

            "status" => request.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(entity => EF.Property<string>(entity, "Status"))
                : query.OrderBy(entity => EF.Property<string>(entity, "Status")),

            "amount" => request.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(entity => EF.Property<decimal>(entity, "Amount"))
                : query.OrderBy(entity => EF.Property<decimal>(entity, "Amount")),

            "eventat" => request.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(entity => EF.Property<DateTimeOffset>(entity, "EventAtUtc"))
                : query.OrderBy(entity => EF.Property<DateTimeOffset>(entity, "EventAtUtc")),

            _ => request.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderBy(entity => EF.Property<DateTimeOffset>(entity, "CreatedAtUtc"))
                : query.OrderByDescending(entity => EF.Property<DateTimeOffset>(entity, "CreatedAtUtc"))
        };

        // =====================================================
        // PAGINATION
        // =====================================================

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<OperationalResponse>
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

    public async Task<OperationalResponse> GetByIdAsync(
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
    // GET BY REFERENCE NUMBER
    // =========================================================

    public async Task<OperationalResponse> GetByReferenceNumberAsync(
        string referenceNumber,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;
        var normalizedReferenceNumber = referenceNumber.Trim().ToUpperInvariant();

        var entity = await db.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                entity =>
                    EF.Property<Guid>(entity, "HotelId") == hotelId &&
                    EF.Property<string>(entity, "ReferenceNumber") == normalizedReferenceNumber &&
                    !EF.Property<bool>(entity, "IsDeleted"),
                cancellationToken)
            ?? throw new NotFoundException($"{typeof(TEntity).Name} not found.");

        return ToResponse(entity);
    }

    // =========================================================
    // GET BY RELATED ENTITY
    // =========================================================

    public async Task<IReadOnlyList<OperationalResponse>> GetByRelatedEntityAsync(
        Guid relatedEntityId,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var items = await db.Set<TEntity>()
            .AsNoTracking()
            .Where(entity =>
                EF.Property<Guid>(entity, "HotelId") == hotelId &&
                EF.Property<Guid?>(entity, "RelatedEntityId") == relatedEntityId &&
                !EF.Property<bool>(entity, "IsDeleted"))
            .OrderByDescending(entity => EF.Property<DateTimeOffset>(entity, "CreatedAtUtc"))
            .ToListAsync(cancellationToken);

        return items.Select(ToResponse).ToList();
    }

    // =========================================================
    // COUNT
    // =========================================================

    public Task<int> CountAsync(
        string? status,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var query = db.Set<TEntity>()
            .AsNoTracking()
            .Where(entity =>
                EF.Property<Guid>(entity, "HotelId") == hotelId &&
                !EF.Property<bool>(entity, "IsDeleted"));

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(entity =>
                EF.Property<string>(entity, "Status") == status);
        }

        return query.CountAsync(cancellationToken);
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<OperationalResponse> CreateAsync(
        OperationalCreateRequest request,
        Func<Guid, Guid?, string, OperationalCreateRequest, TEntity> factory,
        string prefix,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var referenceNumber =
            $"{prefix}-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

        var entity = factory(
            hotelId,
            request.BranchId ?? currentUser.BranchId,
            referenceNumber,
            request);

        db.Set<TEntity>().Add(entity);

        await db.SaveChangesAsync(cancellationToken);

        return ToResponse(entity);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<OperationalResponse> UpdateAsync(
        Guid id,
        OperationalUpdateRequest request,
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

        entity.UpdateRecord(
            request.Title,
            request.Notes,
            request.Amount,
            request.EventAtUtc,
            request.RelatedEntityId,
            request.RelatedEntityType);

        await db.SaveChangesAsync(cancellationToken);

        return ToResponse(entity);
    }

    // =========================================================
    // CHANGE STATUS
    // =========================================================

    public async Task<OperationalResponse> ChangeStatusAsync(
        Guid id,
        string status,
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

        entity.ChangeStatus(status);

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
    // RESPONSE
    // =========================================================

    private static OperationalResponse ToResponse(TEntity entity) =>
        new(
            entity.Id,
            entity.HotelId,
            entity.BranchId,
            entity.ReferenceNumber,
            entity.Title,
            entity.Status,
            entity.Notes,
            entity.Amount,
            entity.EventAtUtc,
            entity.RelatedEntityId,
            entity.RelatedEntityType,
            entity.CreatedAtUtc);
}