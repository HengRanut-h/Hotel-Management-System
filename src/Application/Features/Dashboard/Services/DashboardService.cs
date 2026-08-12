using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Domain.Modules.Billing.Enums;
using HotelManagement.Domain.Modules.Reservations.Enums;
using HotelManagement.Domain.Modules.Rooms.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Dashboard.Services;

public sealed class DashboardService(
    IApplicationDbContext db)
{
    // =========================================================
    // GET DASHBOARD
    // =========================================================

    public async Task<object> GetAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        // =====================================================
        // BASE QUERIES
        // =====================================================

        var roomsQuery =
            db.Rooms
                .AsNoTracking()
                .Where(
                    room =>
                        room.HotelId == hotelId
                        &&
                        !room.IsDeleted);

        var reservationsQuery =
            db.Reservations
                .AsNoTracking()
                .Where(
                    reservation =>
                        reservation.HotelId == hotelId
                        &&
                        !reservation.IsDeleted);

        var invoicesQuery =
            db.Invoices
                .AsNoTracking()
                .Where(
                    invoice =>
                        invoice.HotelId == hotelId
                        &&
                        !invoice.IsDeleted);

        // =====================================================
        // CURRENT DATE
        // =====================================================

        var today =
            DateOnly.FromDateTime(
                DateTime.UtcNow);

        // =====================================================
        // ROOM SUMMARY
        // =====================================================

        var totalRooms =
            await roomsQuery.CountAsync(
                cancellationToken);

        var availableRooms =
            await roomsQuery.CountAsync(
                room =>
                    room.Status == RoomStatus.Available,
                cancellationToken);

        var occupiedRooms =
            await roomsQuery.CountAsync(
                room =>
                    room.Status == RoomStatus.Occupied,
                cancellationToken);

        var dirtyRooms =
            await roomsQuery.CountAsync(
                room =>
                    room.Status == RoomStatus.Dirty,
                cancellationToken);

        // =====================================================
        // RESERVATION SUMMARY
        // =====================================================

        var arrivalsToday =
            await reservationsQuery.CountAsync(
                reservation =>
                    reservation.CheckInDate == today
                    &&
                    reservation.Status == ReservationStatus.Confirmed,
                cancellationToken);

        var departuresToday =
            await reservationsQuery.CountAsync(
                reservation =>
                    reservation.CheckOutDate == today
                    &&
                    reservation.Status == ReservationStatus.CheckedIn,
                cancellationToken);

        var currentGuests =
            await reservationsQuery.CountAsync(
                reservation =>
                    reservation.Status == ReservationStatus.CheckedIn,
                cancellationToken);

        // =====================================================
        // BILLING SUMMARY
        // =====================================================

        var outstandingBalance =
            await invoicesQuery
                .Where(
                    invoice =>
                        invoice.Status != InvoiceStatus.Paid
                        &&
                        invoice.Status != InvoiceStatus.Cancelled)
                .SumAsync(
                    invoice =>
                        invoice.TotalAmount -
                        invoice.PaidAmount,
                    cancellationToken);

        // =====================================================
        // OCCUPANCY
        // =====================================================

        var occupancyRate =
            totalRooms == 0
                ? 0
                : Math.Round(
                    occupiedRooms * 100d / totalRooms,
                    2);

        // =====================================================
        // RESPONSE
        // =====================================================

        return new
        {
            date = today,

            totalRooms,
            availableRooms,
            occupiedRooms,
            dirtyRooms,

            occupancyRate,

            arrivalsToday,
            departuresToday,
            currentGuests,

            outstandingBalance
        };
    }
}