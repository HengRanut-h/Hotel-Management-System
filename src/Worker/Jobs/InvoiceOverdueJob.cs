using HotelManagement.Domain.Modules.Billing.Enums;
using HotelManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Worker.Jobs;

public sealed class InvoiceOverdueJob(IServiceScopeFactory scopes, ILogger<InvoiceOverdueJob> log) : BackgroundService
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
                var items = await db.Invoices.Where(x =>
                        x.DueDate != null && x.DueDate < today &&
                        (x.Status == InvoiceStatus.Issued || x.Status == InvoiceStatus.PartiallyPaid) && !x.IsDeleted)
                    .ToListAsync(token);
                foreach (var x in items) x.MarkOverdue();
                if (items.Count > 0) await db.SaveChangesAsync(token);
                log.LogInformation("InvoiceOverdueJob processed {Count} invoices", items.Count);
            }
            catch (OperationCanceledException)when (token.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "InvoiceOverdueJob failed");
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