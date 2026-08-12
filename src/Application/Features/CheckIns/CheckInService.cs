using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Domain.Modules.FrontDesk.Entities;
using HotelManagement.Domain.Modules.Reservations.Enums;
using HotelManagement.Domain.Modules.Rooms.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.CheckIns;

public sealed record CheckInCommand(
    Guid RoomId);

public sealed class CheckInService(
    IApplicationDbContext db,
    ICurrentUser currentUser)
{
    // =========================================================
    // CHECK IN
    // =========================================================

    public async Task<CheckInRecord> ExecuteAsync(
        Guid reservationId,
        CheckInCommand request,
        CancellationToken cancellationToken)
    {
        // =====================================================
        // HOTEL CONTEXT
        // =====================================================

        var hotelId =
            currentUser.HotelId
            ?? throw new ForbiddenException(
                "Hotel context required.");

        // =====================================================
        // RESERVATION
        // =====================================================

        var reservation =
            await db.Reservations
                .FirstOrDefaultAsync(
                    reservation =>
                        reservation.HotelId == hotelId
                        &&
                        reservation.Id == reservationId
                        &&
                        !reservation.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Reservation not found.");

        // =====================================================
        // RESERVATION STATUS VALIDATION
        // =====================================================

        if (reservation.Status != ReservationStatus.Confirmed)
        {
            throw new ConflictException(
                "Reservation is not ready for check-in.");
        }

        // =====================================================
        // ROOM
        // =====================================================

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

        // =====================================================
        // ROOM AVAILABILITY VALIDATION
        // =====================================================

        if (
            room.Status is
                RoomStatus.Occupied
                or RoomStatus.Maintenance
                or RoomStatus.OutOfOrder
                or RoomStatus.Blocked)
        {
            throw new ConflictException(
                "Room is not available for check-in.");
        }

        // =====================================================
        // CHECK-IN RESERVATION
        // =====================================================

        reservation.CheckIn(
            room.Id);

        // =====================================================
        // UPDATE ROOM STATUS
        // =====================================================

        room.ChangeStatus(
            RoomStatus.Occupied);

        // =====================================================
        // CREATE CHECK-IN RECORD
        // =====================================================

        var checkInRecord =
            new CheckInRecord(
                hotelId,
                reservation.BranchId,
                reservation.Id,
                room.Id,
                reservation.GuestId,
                DateTimeOffset.UtcNow,
                currentUser.UserId);

        db.CheckInRecords.Add(
            checkInRecord);

        // =====================================================
        // SAVE
        // =====================================================

        await db.SaveChangesAsync(
            cancellationToken);

        return checkInRecord;
    }
}