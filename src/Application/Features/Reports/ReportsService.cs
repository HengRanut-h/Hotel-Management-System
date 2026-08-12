using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Domain.Modules.Reservations.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Reports;

public sealed class ReportsService(
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
    // REVENUE REPORT
    // =========================================================

    public async Task<object> RevenueAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken)
    {
        if (to < from)
        {
            throw new ConflictException(
                "The 'to' date cannot be earlier than the 'from' date.");
        }

        var hotelId = HotelId;

        var fromUtc =
            from.ToDateTime(
                TimeOnly.MinValue,
                DateTimeKind.Utc);

        var toUtc =
            to
                .AddDays(1)
                .ToDateTime(
                    TimeOnly.MinValue,
                    DateTimeKind.Utc);

        var payments =
            await db.Payments
                .AsNoTracking()
                .Where(
                    payment =>
                        payment.HotelId == hotelId
                        &&
                        payment.PaidAtUtc >= fromUtc
                        &&
                        payment.PaidAtUtc < toUtc
                        &&
                        !payment.IsDeleted)
                .ToListAsync(
                    cancellationToken);

        var totalAmount =
            payments.Sum(
                payment =>
                    payment.Amount);

        var paymentCount =
            payments.Count;

        var byMethod =
            payments
                .GroupBy(
                    payment =>
                        payment.Method)
                .Select(
                    group =>
                        new
                        {
                            method =
                                group.Key.ToString(),

                            amount =
                                group.Sum(
                                    payment =>
                                        payment.Amount),

                            count =
                                group.Count()
                        })
                .OrderByDescending(
                    item =>
                        item.amount)
                .ToList();

        return new
        {
            from,
            to,
            total = totalAmount,
            count = paymentCount,
            byMethod
        };
    }

    // =========================================================
    // OCCUPANCY REPORT
    // =========================================================

    public async Task<object> OccupancyAsync(
        DateOnly date,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var totalRooms =
            await db.Rooms.CountAsync(
                room =>
                    room.HotelId == hotelId
                    &&
                    !room.IsDeleted,
                cancellationToken);

        var occupiedRooms =
            await db.Reservations
                .Where(
                    reservation =>
                        reservation.HotelId == hotelId
                        &&
                        reservation.Status == ReservationStatus.CheckedIn
                        &&
                        reservation.RoomId != null
                        &&
                        !reservation.IsDeleted)
                .Select(
                    reservation =>
                        reservation.RoomId!.Value)
                .Distinct()
                .CountAsync(
                    cancellationToken);

        var occupancyRate =
            totalRooms == 0
                ? 0
                : Math.Round(
                    occupiedRooms *
                    100d /
                    totalRooms,
                    2);

        return new
        {
            date,
            totalRooms,
            occupiedRooms,
            occupancyRate
        };
    }
}