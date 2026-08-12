using AutoMapper;
using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Application.Features.Rooms.Contracts;
using HotelManagement.Domain.Modules.Rooms.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Rooms.Services;

public sealed class RoomService(
    IApplicationDbContext db,
    IMapper mapper)
{
    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<PagedResult<RoomResponse>> GetAllAsync(
        Guid hotelId,
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        pageNumber = Math.Max(
            1,
            pageNumber);

        pageSize = Math.Clamp(
            pageSize,
            1,
            100);

        var query =
            db.Rooms
                .AsNoTracking()
                .Include(
                    room =>
                        room.RoomType)
                .Where(
                    room =>
                        room.HotelId == hotelId
                        &&
                        !room.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm =
                search.Trim();

            query =
                query.Where(
                    room =>
                        room.RoomNumber.Contains(
                            searchTerm)
                        ||
                        room.RoomType.Name.Contains(
                            searchTerm));
        }

        var totalItems =
            await query.CountAsync(
                cancellationToken);

        var entities =
            await query
                .OrderBy(
                    room =>
                        room.RoomNumber)
                .Skip(
                    (pageNumber - 1) *
                    pageSize)
                .Take(
                    pageSize)
                .ToListAsync(
                    cancellationToken);

        return new PagedResult<RoomResponse>
        {
            Items =
                mapper.Map<List<RoomResponse>>(
                    entities),

            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<RoomResponse> GetByIdAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var room =
            await db.Rooms
                .AsNoTracking()
                .Include(
                    room =>
                        room.RoomType)
                .FirstOrDefaultAsync(
                    room =>
                        room.HotelId == hotelId
                        &&
                        room.Id == id
                        &&
                        !room.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Room not found.");

        return mapper.Map<RoomResponse>(
            room);
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<RoomResponse> CreateAsync(
        Guid hotelId,
        Guid defaultBranchId,
        CreateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var branchId =
            request.BranchId
            ?? defaultBranchId;

        var branchExists =
            await db.HotelBranches.AnyAsync(
                branch =>
                    branch.Id == branchId
                    &&
                    branch.HotelId == hotelId
                    &&
                    !branch.IsDeleted,
                cancellationToken);

        if (!branchExists)
        {
            throw new NotFoundException(
                "Branch not found.");
        }

        var roomTypeExists =
            await db.RoomTypes.AnyAsync(
                roomType =>
                    roomType.Id == request.RoomTypeId
                    &&
                    roomType.HotelId == hotelId
                    &&
                    !roomType.IsDeleted,
                cancellationToken);

        if (!roomTypeExists)
        {
            throw new NotFoundException(
                "Room type not found.");
        }

        var roomNumber =
            request.RoomNumber.Trim();

        var duplicateExists =
            await db.Rooms.AnyAsync(
                room =>
                    room.HotelId == hotelId
                    &&
                    room.BranchId == branchId
                    &&
                    room.RoomNumber == roomNumber
                    &&
                    !room.IsDeleted,
                cancellationToken);

        if (duplicateExists)
        {
            throw new ConflictException(
                "Room number already exists in this branch.");
        }

        var room =
            new Room(
                hotelId,
                branchId,
                request.RoomTypeId,
                roomNumber,
                request.Floor);

        db.Rooms.Add(
            room);

        await db.SaveChangesAsync(
            cancellationToken);

        return await GetByIdAsync(
            hotelId,
            room.Id,
            cancellationToken);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<RoomResponse> UpdateAsync(
        Guid hotelId,
        Guid id,
        UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var room =
            await db.Rooms
                .FirstOrDefaultAsync(
                    room =>
                        room.HotelId == hotelId
                        &&
                        room.Id == id
                        &&
                        !room.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Room not found.");

        var roomTypeExists =
            await db.RoomTypes.AnyAsync(
                roomType =>
                    roomType.Id == request.RoomTypeId
                    &&
                    roomType.HotelId == hotelId
                    &&
                    !roomType.IsDeleted,
                cancellationToken);

        if (!roomTypeExists)
        {
            throw new NotFoundException(
                "Room type not found.");
        }

        var roomNumber =
            request.RoomNumber.Trim();

        var duplicateExists =
            await db.Rooms.AnyAsync(
                existingRoom =>
                    existingRoom.HotelId == hotelId
                    &&
                    existingRoom.BranchId == room.BranchId
                    &&
                    existingRoom.RoomNumber == roomNumber
                    &&
                    existingRoom.Id != id
                    &&
                    !existingRoom.IsDeleted,
                cancellationToken);

        if (duplicateExists)
        {
            throw new ConflictException(
                "Room number already exists in this branch.");
        }

        room.Update(
            request.RoomTypeId,
            roomNumber,
            request.Floor);

        room.MarkUpdated();

        await db.SaveChangesAsync(
            cancellationToken);

        return await GetByIdAsync(
            hotelId,
            id,
            cancellationToken);
    }

    // =========================================================
    // CHANGE STATUS
    // =========================================================

    public async Task ChangeStatusAsync(
        Guid hotelId,
        Guid id,
        ChangeRoomStatusRequest request,
        CancellationToken cancellationToken)
    {
        var room =
            await db.Rooms
                .FirstOrDefaultAsync(
                    room =>
                        room.HotelId == hotelId
                        &&
                        room.Id == id
                        &&
                        !room.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Room not found.");

        room.ChangeStatus(
            request.Status);

        room.MarkUpdated();

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // SOFT DELETE
    // =========================================================

    public async Task DeleteAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var room =
            await db.Rooms
                .FirstOrDefaultAsync(
                    room =>
                        room.HotelId == hotelId
                        &&
                        room.Id == id
                        &&
                        !room.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Room not found.");

        room.SoftDelete();

        await db.SaveChangesAsync(
            cancellationToken);
    }
}