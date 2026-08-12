using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Domain.Modules.Hotels.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Hotels;

public sealed record HotelRequest(
    string Name,
    string Code,
    string Currency = "USD",
    bool IsActive = true);

public sealed record HotelResponse(
    Guid Id,
    string Name,
    string Code,
    string Currency,
    bool IsActive,
    DateTimeOffset CreatedAtUtc);

public sealed class HotelsService(
    IApplicationDbContext db)
{
    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<PagedResult<HotelResponse>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        pageNumber =
            Math.Max(
                1,
                pageNumber);

        pageSize =
            Math.Clamp(
                pageSize,
                1,
                100);

        var query =
            db.Hotels
                .AsNoTracking()
                .Where(
                    hotel =>
                        !hotel.IsDeleted);

        // =====================================================
        // SEARCH
        // =====================================================

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm =
                search.Trim();

            query =
                query.Where(
                    hotel =>
                        hotel.Name.Contains(searchTerm)
                        ||
                        hotel.Code.Contains(searchTerm));
        }

        // =====================================================
        // TOTAL
        // =====================================================

        var totalItems =
            await query.CountAsync(
                cancellationToken);

        // =====================================================
        // ITEMS
        // =====================================================

        var items =
            await query
                .OrderBy(
                    hotel =>
                        hotel.Name)
                .Skip(
                    (pageNumber - 1) *
                    pageSize)
                .Take(
                    pageSize)
                .Select(
                    hotel =>
                        new HotelResponse(
                            hotel.Id,
                            hotel.Name,
                            hotel.Code,
                            hotel.Currency,
                            hotel.IsActive,
                            hotel.CreatedAtUtc))
                .ToListAsync(
                    cancellationToken);

        return new PagedResult<HotelResponse>
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

    public async Task<HotelResponse> GetAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotel =
            await db.Hotels
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    hotel =>
                        hotel.Id == id
                        &&
                        !hotel.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Hotel not found.");

        return ToResponse(
            hotel);
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<HotelResponse> CreateAsync(
        HotelRequest request,
        CancellationToken cancellationToken)
    {
        var code =
            request.Code
                .Trim()
                .ToUpperInvariant();

        var codeExists =
            await db.Hotels.AnyAsync(
                hotel =>
                    hotel.Code == code
                    &&
                    !hotel.IsDeleted,
                cancellationToken);

        if (codeExists)
        {
            throw new ConflictException(
                "Hotel code already exists.");
        }

        var hotel =
            new Hotel(
                request.Name,
                code,
                request.Currency);

        hotel.SetActive(
            request.IsActive);

        db.Hotels.Add(
            hotel);

        await db.SaveChangesAsync(
            cancellationToken);

        return await GetAsync(
            hotel.Id,
            cancellationToken);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<HotelResponse> UpdateAsync(
        Guid id,
        HotelRequest request,
        CancellationToken cancellationToken)
    {
        var hotel =
            await db.Hotels
                .FirstOrDefaultAsync(
                    hotel =>
                        hotel.Id == id
                        &&
                        !hotel.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Hotel not found.");

        var code =
            request.Code
                .Trim()
                .ToUpperInvariant();

        var codeExists =
            await db.Hotels.AnyAsync(
                existingHotel =>
                    existingHotel.Id != id
                    &&
                    existingHotel.Code == code
                    &&
                    !existingHotel.IsDeleted,
                cancellationToken);

        if (codeExists)
        {
            throw new ConflictException(
                "Hotel code already exists.");
        }

        hotel.Update(
            request.Name,
            code,
            request.Currency);

        hotel.SetActive(
            request.IsActive);

        await db.SaveChangesAsync(
            cancellationToken);

        return await GetAsync(
            id,
            cancellationToken);
    }

    // =========================================================
    // ACTIVE / INACTIVE
    // =========================================================

    public async Task<HotelResponse> SetActiveAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var hotel =
            await db.Hotels
                .FirstOrDefaultAsync(
                    hotel =>
                        hotel.Id == id
                        &&
                        !hotel.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Hotel not found.");

        hotel.SetActive(
            isActive);

        await db.SaveChangesAsync(
            cancellationToken);

        return await GetAsync(
            id,
            cancellationToken);
    }

    // =========================================================
    // SOFT DELETE
    // =========================================================

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotel =
            await db.Hotels
                .FirstOrDefaultAsync(
                    hotel =>
                        hotel.Id == id
                        &&
                        !hotel.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Hotel not found.");

        hotel.SoftDelete();

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // MAPPING
    // =========================================================

    private static HotelResponse ToResponse(
        Hotel hotel) =>
        new(
            hotel.Id,
            hotel.Name,
            hotel.Code,
            hotel.Currency,
            hotel.IsActive,
            hotel.CreatedAtUtc);
}