using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Features.Maintenance.Contracts;
using HotelManagement.Domain.Modules.Maintenance.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Maintenance.Services;

public sealed class MaintenanceService(
    IApplicationDbContext db)
{
    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<List<object>> GetAllAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        return await db.MaintenanceRequests
            .AsNoTracking()
            .Where(
                maintenanceRequest =>
                    !maintenanceRequest.IsDeleted
                    &&
                    (
                        maintenanceRequest.RoomId == null
                        ||
                        maintenanceRequest.Room!.HotelId == hotelId
                    ))
            .OrderByDescending(
                maintenanceRequest =>
                    maintenanceRequest.CreatedAtUtc)
            .Select(
                maintenanceRequest =>
                    (object)new
                    {
                        maintenanceRequest.Id,
                        maintenanceRequest.RoomId,

                        RoomNumber =
                            maintenanceRequest.Room != null
                                ? maintenanceRequest.Room.RoomNumber
                                : null,

                        maintenanceRequest.Category,
                        maintenanceRequest.Description,
                        maintenanceRequest.Priority,
                        maintenanceRequest.Status,
                        maintenanceRequest.AssignedUserId,
                        maintenanceRequest.CreatedAtUtc
                    })
            .ToListAsync(
                cancellationToken);
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<object> GetByIdAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken)
    {
        return await db.MaintenanceRequests
            .AsNoTracking()
            .Where(
                maintenanceRequest =>
                    maintenanceRequest.Id == id
                    &&
                    !maintenanceRequest.IsDeleted
                    &&
                    (
                        maintenanceRequest.RoomId == null
                        ||
                        maintenanceRequest.Room!.HotelId == hotelId
                    ))
            .Select(
                maintenanceRequest =>
                    (object)new
                    {
                        maintenanceRequest.Id,
                        maintenanceRequest.RoomId,

                        RoomNumber =
                            maintenanceRequest.Room != null
                                ? maintenanceRequest.Room.RoomNumber
                                : null,

                        maintenanceRequest.Category,
                        maintenanceRequest.Description,
                        maintenanceRequest.Priority,
                        maintenanceRequest.Status,
                        maintenanceRequest.AssignedUserId,
                        maintenanceRequest.CreatedAtUtc
                    })
            .FirstOrDefaultAsync(
                cancellationToken)
            ?? throw new NotFoundException(
                "Maintenance request not found.");
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<Guid> CreateAsync(
        Guid hotelId,
        CreateMaintenanceRequest request,
        CancellationToken cancellationToken)
    {
        if (request.RoomId.HasValue)
        {
            var roomExists =
                await db.Rooms.AnyAsync(
                    room =>
                        room.Id == request.RoomId.Value
                        &&
                        room.HotelId == hotelId
                        &&
                        !room.IsDeleted,
                    cancellationToken);

            if (!roomExists)
            {
                throw new NotFoundException(
                    "Room not found.");
            }
        }

        var maintenanceRequest =
            new MaintenanceRequest(
                request.RoomId,
                request.Category,
                request.Description,
                request.Priority);

        db.MaintenanceRequests.Add(
            maintenanceRequest);

        await db.SaveChangesAsync(
            cancellationToken);

        return maintenanceRequest.Id;
    }

    // =========================================================
    // COMPLETE
    // =========================================================

    public async Task CompleteAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var maintenanceRequest =
            await db.MaintenanceRequests
                .Include(
                    maintenanceRequest =>
                        maintenanceRequest.Room)
                .FirstOrDefaultAsync(
                    maintenanceRequest =>
                        maintenanceRequest.Id == id
                        &&
                        !maintenanceRequest.IsDeleted
                        &&
                        (
                            maintenanceRequest.RoomId == null
                            ||
                            maintenanceRequest.Room!.HotelId == hotelId
                        ),
                    cancellationToken)
            ?? throw new NotFoundException(
                "Maintenance request not found.");

        maintenanceRequest.Complete();

        await db.SaveChangesAsync(
            cancellationToken);
    }
}