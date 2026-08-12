using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Domain.Modules.Notifications.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Notifications;

public sealed class NotificationsService(
    IApplicationDbContext db,
    ICurrentUser currentUser)
{
    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<List<Notification>> GetAsync(
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.HotelId
            ?? throw new ForbiddenException(
                "Hotel context required.");

        return await db.Notifications
            .AsNoTracking()
            .Where(
                notification =>
                    notification.HotelId == hotelId
                    &&
                    (
                        notification.UserId == null
                        ||
                        notification.UserId == currentUser.UserId
                    )
                    &&
                    !notification.IsDeleted)
            .OrderByDescending(
                notification =>
                    notification.CreatedAtUtc)
            .Take(100)
            .ToListAsync(
                cancellationToken);
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<Notification> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.HotelId
            ?? throw new ForbiddenException(
                "Hotel context required.");

        return await db.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(
                notification =>
                    notification.Id == id
                    &&
                    notification.HotelId == hotelId
                    &&
                    (
                        notification.UserId == null
                        ||
                        notification.UserId == currentUser.UserId
                    )
                    &&
                    !notification.IsDeleted,
                cancellationToken)
            ?? throw new NotFoundException(
                "Notification not found.");
    }

    // =========================================================
    // MARK AS READ
    // =========================================================

    public async Task ReadAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.HotelId
            ?? throw new ForbiddenException(
                "Hotel context required.");

        var notification =
            await db.Notifications
                .FirstOrDefaultAsync(
                    notification =>
                        notification.Id == id
                        &&
                        notification.HotelId == hotelId
                        &&
                        notification.UserId == currentUser.UserId
                        &&
                        !notification.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Notification not found.");

        notification.MarkRead();

        await db.SaveChangesAsync(
            cancellationToken);
    }
}