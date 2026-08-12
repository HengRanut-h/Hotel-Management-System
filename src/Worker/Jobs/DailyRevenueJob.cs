using HotelManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Worker.Jobs;

public sealed class DailyRevenueJob(IServiceScopeFactory scopes, ILogger<DailyRevenueJob> log) : BackgroundService
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
                var start = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero);
                var end = start.AddDays(1);
                var total = await db.Payments.Where(x => x.PaidAtUtc >= start && x.PaidAtUtc < end && !x.IsDeleted)
                    .SumAsync(x => (decimal?)x.Amount, token) ?? 0m;
                log.LogInformation("Daily revenue {Date}: {Total}", start, total);
            }
            catch (Exception ex)when (ex is not OperationCanceledException)
            {
                log.LogError(ex, "DailyRevenueJob failed");
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