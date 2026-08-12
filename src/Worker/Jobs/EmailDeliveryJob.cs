namespace HotelManagement.Worker.Jobs;

public sealed class EmailDeliveryJob(IServiceScopeFactory scopes, ILogger<EmailDeliveryJob> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(30));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                log.LogInformation("EmailDeliveryJob: Process queued email delivery work.");
                using var scope = scopes.CreateScope();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "EmailDeliveryJob failed");
            }

            try
            {
                if (!await timer.WaitForNextTickAsync(stoppingToken)) break;
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}