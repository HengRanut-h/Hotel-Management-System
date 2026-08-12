using HotelManagement.Domain.Modules.Reservations.Enums;
using HotelManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Worker.Jobs;

public sealed class ReservationReminderJob(IServiceScopeFactory scopes, ILogger<ReservationReminderJob> log)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(6));
        do
        {
            try
            {
                using var scope = scopes.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
                var count = await db.Reservations.CountAsync(
                    x => x.CheckInDate == tomorrow && x.Status == ReservationStatus.Confirmed && !x.IsDeleted, token);
                log.LogInformation("ReservationReminderJob found {Count} arrivals for {Date}", count, tomorrow);
            }
            catch (Exception ex)when (ex is not OperationCanceledException)
            {
                log.LogError(ex, "ReservationReminderJob failed");
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