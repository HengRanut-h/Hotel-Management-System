using HotelManagement.Domain.Modules.Reservations.Enums;
using HotelManagement.Domain.Modules.Rooms.Enums;
using HotelManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Worker.Jobs;

public sealed class NoShowProcessingJob(IServiceScopeFactory scopes, ILogger<NoShowProcessingJob> log)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        do
        {
            try
            {
                using var scope = scopes.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var items = await db.Reservations.Include(x => x.Room).Where(x =>
                        x.CheckInDate < today && x.Status == ReservationStatus.Confirmed && !x.IsDeleted)
                    .ToListAsync(token);
                foreach (var x in items)
                {
                    x.MarkNoShow();
                    if (x.Room != null) x.Room.ChangeStatus(RoomStatus.Available);
                }

                if (items.Count > 0) await db.SaveChangesAsync(token);
                log.LogInformation("NoShowProcessingJob processed {Count} reservations", items.Count);
            }
            catch (OperationCanceledException)when (token.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "NoShowProcessingJob failed");
            }

            try
            {
                if (!await timer.WaitForNextTickAsync(token)) break;
            }
            catch (OperationCanceledException)
            {
                break;
            }
        } while (!token.IsCancellationRequested);
    }
}