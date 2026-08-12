using HotelManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Worker.Jobs;

public sealed class UtilityBillingJob(IServiceScopeFactory scopes, ILogger<UtilityBillingJob> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(12));
        do
        {
            try
            {
                using var scope = scopes.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var monthStart = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                var count = await db.MeterReadings.CountAsync(x => x.ReadingDate >= monthStart && !x.IsDeleted, token);
                var total = await db.MeterReadings.Where(x => x.ReadingDate >= monthStart && !x.IsDeleted)
                    .SumAsync(x => (decimal?)x.Amount, token) ?? 0m;
                log.LogInformation("UtilityBillingJob month readings {Count}, calculated amount {Total}", count, total);
            }
            catch (Exception ex)when (ex is not OperationCanceledException)
            {
                log.LogError(ex, "UtilityBillingJob failed");
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