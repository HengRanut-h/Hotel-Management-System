using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Domain.Modules.FrontDesk.Entities;
using HotelManagement.Domain.Modules.Reservations.Enums;
using HotelManagement.Domain.Modules.Rooms.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.CheckOuts;

public sealed class CheckOutService(
    IApplicationDbContext db,
    ICurrentUser currentUser)
{
    // =========================================================
    // CHECK OUT
    // =========================================================

    public async Task<CheckOutRecord> ExecuteAsync(
        Guid reservationId,
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
                .Include(
                    reservation =>
                        reservation.Room)
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

        if (reservation.Status != ReservationStatus.CheckedIn)
        {
            throw new ConflictException(
                "Reservation is not checked in.");
        }

        // =====================================================
        // CURRENT ROOM
        // =====================================================

        var roomId =
            reservation.RoomId;

        // =====================================================
        // CHECK OUT RESERVATION
        // =====================================================

        reservation.CheckOut();

        // =====================================================
        // UPDATE ROOM STATUS
        // =====================================================

        if (reservation.Room is not null)
        {
            reservation.Room.ChangeStatus(
                RoomStatus.Dirty);
        }

        // =====================================================
        // CREATE CHECK-OUT RECORD
        // =====================================================

        var checkOutRecord =
            new CheckOutRecord(
                hotelId,
                reservation.BranchId,
                reservation.Id,
                roomId,
                reservation.GuestId,
                DateTimeOffset.UtcNow,
                currentUser.UserId);

        db.CheckOutRecords.Add(
            checkOutRecord);

        // =====================================================
        // SAVE
        // =====================================================

        await db.SaveChangesAsync(
            cancellationToken);

        return checkOutRecord;
    }
}