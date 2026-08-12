using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Domain.Modules.Rooms.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.RoomTypes;

public sealed record RoomTypeRequest(
    string Name,
    string Code,
    decimal BaseRate,
    int MaxAdults,
    int MaxChildren,
    string? Description = null,
    bool IsActive = true);

public sealed record RoomTypeResponse(
    Guid Id,
    string Name,
    string Code,
    decimal BaseRate,
    int MaxAdults,
    int MaxChildren,
    string? Description,
    bool IsActive);

public sealed class RoomTypesService(
    IApplicationDbContext db,
    ICurrentUser currentUser)
{
    // =========================================================
    // HOTEL CONTEXT
    // =========================================================

    private Guid HotelId =>
        currentUser.HotelId
        ?? throw new ForbiddenException(
            "Hotel context required.");

    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<PagedResult<RoomTypeResponse>> GetAllAsync(
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

        var hotelId = HotelId;

        var query =
            db.RoomTypes
                .AsNoTracking()
                .Where(
                    roomType =>
                        roomType.HotelId == hotelId
                        &&
                        !roomType.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm =
                search.Trim();

            query =
                query.Where(
                    roomType =>
                        roomType.Name.Contains(searchTerm)
                        ||
                        roomType.Code.Contains(searchTerm));
        }

        var totalItems =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query
                .OrderBy(
                    roomType =>
                        roomType.Name)
                .Skip(
                    (pageNumber - 1) *
                    pageSize)
                .Take(
                    pageSize)
                .Select(
                    roomType =>
                        new RoomTypeResponse(
                            roomType.Id,
                            roomType.Name,
                            roomType.Code,
                            roomType.BaseRate,
                            roomType.MaxAdults,
                            roomType.MaxChildren,
                            roomType.Description,
                            roomType.IsActive))
                .ToListAsync(
                    cancellationToken);

        return new PagedResult<RoomTypeResponse>
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

    public async Task<RoomTypeResponse> GetAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var roomType =
            await db.RoomTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    roomType =>
                        roomType.HotelId == hotelId
                        &&
                        roomType.Id == id
                        &&
                        !roomType.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Room type not found.");

        return ToResponse(
            roomType);
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<RoomTypeResponse> CreateAsync(
        RoomTypeRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var code =
            request.Code
                .Trim()
                .ToUpperInvariant();

        var codeExists =
            await db.RoomTypes.AnyAsync(
                roomType =>
                    roomType.HotelId == hotelId
                    &&
                    roomType.Code == code
                    &&
                    !roomType.IsDeleted,
                cancellationToken);

        if (codeExists)
        {
            throw new ConflictException(
                "Room type code already exists.");
        }

        if (request.BaseRate < 0)
        {
            throw new ConflictException(
                "Base rate cannot be negative.");
        }

        if (request.MaxAdults < 1)
        {
            throw new ConflictException(
                "Maximum adults must be greater than zero.");
        }

        if (request.MaxChildren < 0)
        {
            throw new ConflictException(
                "Maximum children cannot be negative.");
        }

        var roomType =
            new RoomType(
                hotelId,
                request.Name.Trim(),
                code,
                request.BaseRate,
                request.MaxAdults,
                request.MaxChildren,
                request.Description);

        roomType.SetActive(
            request.IsActive);

        db.RoomTypes.Add(
            roomType);

        await db.SaveChangesAsync(
            cancellationToken);

        return await GetAsync(
            roomType.Id,
            cancellationToken);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<RoomTypeResponse> UpdateAsync(
        Guid id,
        RoomTypeRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var roomType =
            await db.RoomTypes
                .FirstOrDefaultAsync(
                    roomType =>
                        roomType.HotelId == hotelId
                        &&
                        roomType.Id == id
                        &&
                        !roomType.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Room type not found.");

        var code =
            request.Code
                .Trim()
                .ToUpperInvariant();

        var duplicateExists =
            await db.RoomTypes.AnyAsync(
                existingRoomType =>
                    existingRoomType.HotelId == hotelId
                    &&
                    existingRoomType.Id != id
                    &&
                    existingRoomType.Code == code
                    &&
                    !existingRoomType.IsDeleted,
                cancellationToken);

        if (duplicateExists)
        {
            throw new ConflictException(
                "Room type code already exists.");
        }

        if (request.BaseRate < 0)
        {
            throw new ConflictException(
                "Base rate cannot be negative.");
        }

        if (request.MaxAdults < 1)
        {
            throw new ConflictException(
                "Maximum adults must be greater than zero.");
        }

        if (request.MaxChildren < 0)
        {
            throw new ConflictException(
                "Maximum children cannot be negative.");
        }

        roomType.Update(
            request.Name.Trim(),
            code,
            request.BaseRate,
            request.MaxAdults,
            request.MaxChildren,
            request.Description);

        roomType.SetActive(
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

    public async Task<RoomTypeResponse> SetActiveAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var roomType =
            await db.RoomTypes
                .FirstOrDefaultAsync(
                    roomType =>
                        roomType.HotelId == hotelId
                        &&
                        roomType.Id == id
                        &&
                        !roomType.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Room type not found.");

        roomType.SetActive(
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
        var hotelId = HotelId;

        var roomType =
            await db.RoomTypes
                .FirstOrDefaultAsync(
                    roomType =>
                        roomType.HotelId == hotelId
                        &&
                        roomType.Id == id
                        &&
                        !roomType.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Room type not found.");

        roomType.SoftDelete(
            currentUser.UserId);

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // MAPPING
    // =========================================================

    private static RoomTypeResponse ToResponse(
        RoomType roomType) =>
        new(
            roomType.Id,
            roomType.Name,
            roomType.Code,
            roomType.BaseRate,
            roomType.MaxAdults,
            roomType.MaxChildren,
            roomType.Description,
            roomType.IsActive);
}