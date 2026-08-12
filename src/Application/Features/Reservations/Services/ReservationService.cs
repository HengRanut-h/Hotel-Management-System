using AutoMapper;
using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Application.Features.Reservations.Contracts;
using HotelManagement.Domain.Modules.Reservations.Entities;
using HotelManagement.Domain.Modules.Reservations.Enums;
using HotelManagement.Domain.Modules.Rooms.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Reservations.Services;

public sealed class ReservationService(
    IApplicationDbContext db,
    IMapper mapper)
{
    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<PagedResult<ReservationResponse>> GetAllAsync(
        Guid hotelId,
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
            db.Reservations
                .AsNoTracking()
                .Include(
                    reservation =>
                        reservation.Guest)
                .Include(
                    reservation =>
                        reservation.RoomType)
                .Include(
                    reservation =>
                        reservation.Room)
                .Where(
                    reservation =>
                        reservation.HotelId == hotelId
                        &&
                        !reservation.IsDeleted);

        // =====================================================
        // SEARCH
        // =====================================================

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm =
                search.Trim();

            query =
                query.Where(
                    reservation =>
                        reservation.ReservationNumber.Contains(
                            searchTerm)
                        ||
                        reservation.Guest.FirstName.Contains(
                            searchTerm)
                        ||
                        reservation.Guest.LastName.Contains(
                            searchTerm));
        }

        // =====================================================
        // TOTAL
        // =====================================================

        var totalItems =
            await query.CountAsync(
                cancellationToken);

        // =====================================================
        // PAGINATION
        // =====================================================

        var entities =
            await query
                .OrderByDescending(
                    reservation =>
                        reservation.CreatedAtUtc)
                .Skip(
                    (pageNumber - 1) *
                    pageSize)
                .Take(
                    pageSize)
                .ToListAsync(
                    cancellationToken);

        return new PagedResult<ReservationResponse>
        {
            Items =
                mapper.Map<List<ReservationResponse>>(
                    entities),

            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<ReservationResponse> GetByIdAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var reservation =
            await db.Reservations
                .AsNoTracking()
                .Include(
                    reservation =>
                        reservation.Guest)
                .Include(
                    reservation =>
                        reservation.RoomType)
                .Include(
                    reservation =>
                        reservation.Room)
                .FirstOrDefaultAsync(
                    reservation =>
                        reservation.HotelId == hotelId
                        &&
                        reservation.Id == id
                        &&
                        !reservation.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Reservation not found.");

        return mapper.Map<ReservationResponse>(
            reservation);
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<ReservationResponse> CreateAsync(
        Guid hotelId,
        Guid defaultBranchId,
        CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        // =====================================================
        // DATE VALIDATION
        // =====================================================

        if (request.CheckOutDate <= request.CheckInDate)
        {
            throw new ConflictException(
                "Check-out date must be after check-in date.");
        }

        var branchId =
            request.BranchId
            ?? defaultBranchId;

        // =====================================================
        // BRANCH VALIDATION
        // =====================================================

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

        // =====================================================
        // GUEST VALIDATION
        // =====================================================

        var guestExists =
            await db.Guests.AnyAsync(
                guest =>
                    guest.Id == request.GuestId
                    &&
                    guest.HotelId == hotelId
                    &&
                    !guest.IsDeleted,
                cancellationToken);

        if (!guestExists)
        {
            throw new NotFoundException(
                "Guest not found.");
        }

        // =====================================================
        // ROOM TYPE VALIDATION
        // =====================================================

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

        // =====================================================
        // ROOM VALIDATION
        // =====================================================

        if (request.RoomId.HasValue)
        {
            var room =
                await db.Rooms
                    .FirstOrDefaultAsync(
                        room =>
                            room.Id == request.RoomId.Value
                            &&
                            room.HotelId == hotelId
                            &&
                            room.BranchId == branchId
                            &&
                            room.RoomTypeId == request.RoomTypeId
                            &&
                            !room.IsDeleted,
                        cancellationToken)
                ?? throw new NotFoundException(
                    "Room not found.");

            if (
                room.Status is
                    RoomStatus.Occupied
                    or RoomStatus.Maintenance
                    or RoomStatus.OutOfOrder
                    or RoomStatus.Blocked)
            {
                throw new ConflictException(
                    $"Room cannot be reserved while status is {room.Status}.");
            }

            // =================================================
            // OVERLAP VALIDATION
            // =================================================

            var hasOverlap =
                await db.Reservations.AnyAsync(
                    reservation =>
                        reservation.HotelId == hotelId
                        &&
                        reservation.RoomId == request.RoomId.Value
                        &&
                        !reservation.IsDeleted
                        &&
                        reservation.Status != ReservationStatus.Cancelled
                        &&
                        reservation.Status != ReservationStatus.CheckedOut
                        &&
                        request.CheckInDate < reservation.CheckOutDate
                        &&
                        request.CheckOutDate > reservation.CheckInDate,
                    cancellationToken);

            if (hasOverlap)
            {
                throw new ConflictException(
                    "Room is already reserved for the selected dates.");
            }

            room.ChangeStatus(
                RoomStatus.Reserved);
        }

        // =====================================================
        // RESERVATION NUMBER
        // =====================================================

        var reservationNumber =
            $"RES-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

        // =====================================================
        // CREATE RESERVATION
        // =====================================================

        var reservation =
            new Reservation(
                hotelId,
                branchId,
                reservationNumber,
                request.GuestId,
                request.RoomTypeId,
                request.RoomId,
                request.CheckInDate,
                request.CheckOutDate,
                request.Adults,
                request.Children,
                request.NightlyRate);

        db.Reservations.Add(
            reservation);

        await db.SaveChangesAsync(
            cancellationToken);

        return await GetByIdAsync(
            hotelId,
            reservation.Id,
            cancellationToken);
    }

    // =========================================================
    // CANCEL
    // =========================================================

    public async Task CancelAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var reservation =
            await db.Reservations
                .Include(
                    reservation =>
                        reservation.Room)
                .FirstOrDefaultAsync(
                    reservation =>
                        reservation.HotelId == hotelId
                        &&
                        reservation.Id == id
                        &&
                        !reservation.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Reservation not found.");

        reservation.Cancel();

        if (reservation.Room is not null)
        {
            reservation.Room.ChangeStatus(
                RoomStatus.Available);
        }

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // CHECK IN
    // =========================================================

    public async Task CheckInAsync(
        Guid hotelId,
        Guid id,
        CheckInRequest request,
        CancellationToken cancellationToken)
    {
        var reservation =
            await db.Reservations
                .FirstOrDefaultAsync(
                    reservation =>
                        reservation.HotelId == hotelId
                        &&
                        reservation.Id == id
                        &&
                        !reservation.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Reservation not found.");

        var room =
            await db.Rooms
                .FirstOrDefaultAsync(
                    room =>
                        room.HotelId == hotelId
                        &&
                        room.Id == request.RoomId
                        &&
                        !room.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Room not found.");

        if (
            room.Status is
                RoomStatus.Occupied
                or RoomStatus.Maintenance
                or RoomStatus.OutOfOrder
                or RoomStatus.Blocked)
        {
            throw new ConflictException(
                $"Room cannot be checked in while status is {room.Status}.");
        }

        reservation.CheckIn(
            room.Id);

        room.ChangeStatus(
            RoomStatus.Occupied);

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // CHECK OUT
    // =========================================================

    public async Task CheckOutAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var reservation =
            await db.Reservations
                .Include(
                    reservation =>
                        reservation.Room)
                .FirstOrDefaultAsync(
                    reservation =>
                        reservation.HotelId == hotelId
                        &&
                        reservation.Id == id
                        &&
                        !reservation.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Reservation not found.");

        reservation.CheckOut();

        if (reservation.Room is not null)
        {
            reservation.Room.ChangeStatus(
                RoomStatus.Dirty);
        }

        await db.SaveChangesAsync(
            cancellationToken);
    }
}