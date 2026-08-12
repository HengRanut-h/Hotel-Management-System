using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Domain.Modules.Reservations.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Availability;

public sealed record AvailabilityRoom(
    Guid Id,
    string RoomNumber,
    Guid RoomTypeId,
    string RoomTypeName,
    decimal BaseRate,
    string Status);

public sealed class AvailabilityService(
    IApplicationDbContext db,
    ICurrentUser currentUser)
{
    // =========================================================
    // SEARCH AVAILABLE ROOMS
    // =========================================================

    public async Task<List<AvailabilityRoom>> SearchAsync(
        DateOnly checkIn,
        DateOnly checkOut,
        Guid? roomTypeId,
        CancellationToken cancellationToken)
    {
        // =====================================================
        // VALIDATION
        // =====================================================

        if (checkOut <= checkIn)
        {
            throw new ConflictException(
                "Check-out must be after check-in.");
        }

        // =====================================================
        // HOTEL CONTEXT
        // =====================================================

        var hotelId =
            currentUser.HotelId
            ?? throw new ForbiddenException(
                "Hotel context required.");

        // =====================================================
        // BOOKED ROOM IDS
        // =====================================================

        var bookedRoomIds =
            db.Reservations
                .Where(
                    reservation =>
                        reservation.HotelId == hotelId
                        &&
                        reservation.RoomId != null
                        &&
                        reservation.Status != ReservationStatus.Cancelled
                        &&
                        reservation.Status != ReservationStatus.CheckedOut
                        &&
                        checkIn < reservation.CheckOutDate
                        &&
                        checkOut > reservation.CheckInDate)
                .Select(
                    reservation =>
                        reservation.RoomId!.Value);

        // =====================================================
        // AVAILABLE ROOMS QUERY
        // =====================================================

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
                        !room.IsDeleted
                        &&
                        !bookedRoomIds.Contains(room.Id));

        // =====================================================
        // ROOM TYPE FILTER
        // =====================================================

        if (roomTypeId.HasValue)
        {
            query =
                query.Where(
                    room =>
                        room.RoomTypeId == roomTypeId.Value);
        }

        // =====================================================
        // RESPONSE
        // =====================================================

        return await query
            .OrderBy(
                room =>
                    room.RoomNumber)
            .Select(
                room =>
                    new AvailabilityRoom(
                        room.Id,
                        room.RoomNumber,
                        room.RoomTypeId,
                        room.RoomType.Name,
                        room.RoomType.BaseRate,
                        room.Status.ToString()))
            .ToListAsync(
                cancellationToken);
    }
}