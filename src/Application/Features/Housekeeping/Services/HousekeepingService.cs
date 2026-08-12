using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Features.Housekeeping.Contracts;
using HotelManagement.Domain.Modules.Housekeeping.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Housekeeping.Services;

public sealed class HousekeepingService(
    IApplicationDbContext db)
{
    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<List<object>> GetAllAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        return await db.HousekeepingTasks
            .AsNoTracking()
            .Include(
                housekeepingTask =>
                    housekeepingTask.Room)
            .Where(
                housekeepingTask =>
                    housekeepingTask.Room.HotelId == hotelId
                    &&
                    !housekeepingTask.IsDeleted)
            .OrderByDescending(
                housekeepingTask =>
                    housekeepingTask.CreatedAtUtc)
            .Select(
                housekeepingTask =>
                    (object)new
                    {
                        housekeepingTask.Id,
                        housekeepingTask.RoomId,
                        housekeepingTask.Room.RoomNumber,
                        housekeepingTask.TaskType,
                        housekeepingTask.Priority,
                        housekeepingTask.Status,
                        housekeepingTask.AssignedUserId,
                        housekeepingTask.CreatedAtUtc
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
        return await db.HousekeepingTasks
            .AsNoTracking()
            .Include(
                housekeepingTask =>
                    housekeepingTask.Room)
            .Where(
                housekeepingTask =>
                    housekeepingTask.Id == id
                    &&
                    housekeepingTask.Room.HotelId == hotelId
                    &&
                    !housekeepingTask.IsDeleted)
            .Select(
                housekeepingTask =>
                    (object)new
                    {
                        housekeepingTask.Id,
                        housekeepingTask.RoomId,
                        housekeepingTask.Room.RoomNumber,
                        housekeepingTask.TaskType,
                        housekeepingTask.Priority,
                        housekeepingTask.Status,
                        housekeepingTask.AssignedUserId,
                        housekeepingTask.CreatedAtUtc
                    })
            .FirstOrDefaultAsync(
                cancellationToken)
            ?? throw new NotFoundException(
                "Housekeeping task not found.");
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<Guid> CreateAsync(
        Guid hotelId,
        CreateHousekeepingTaskRequest request,
        CancellationToken cancellationToken)
    {
        var room =
            await db.Rooms
                .FirstOrDefaultAsync(
                    room =>
                        room.Id == request.RoomId
                        &&
                        room.HotelId == hotelId
                        &&
                        !room.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Room not found.");

        var housekeepingTask =
            new HousekeepingTask(
                room.Id,
                request.TaskType,
                request.Priority);

        db.HousekeepingTasks.Add(
            housekeepingTask);

        await db.SaveChangesAsync(
            cancellationToken);

        return housekeepingTask.Id;
    }

    // =========================================================
    // COMPLETE
    // =========================================================

    public async Task CompleteAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var housekeepingTask =
            await db.HousekeepingTasks
                .Include(
                    housekeepingTask =>
                        housekeepingTask.Room)
                .FirstOrDefaultAsync(
                    housekeepingTask =>
                        housekeepingTask.Id == id
                        &&
                        housekeepingTask.Room.HotelId == hotelId
                        &&
                        !housekeepingTask.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Housekeeping task not found.");

        housekeepingTask.Complete();

        await db.SaveChangesAsync(
            cancellationToken);
    }
}