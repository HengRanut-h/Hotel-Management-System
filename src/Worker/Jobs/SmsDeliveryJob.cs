namespace HotelManagement.Worker.Jobs;

public sealed class SmsDeliveryJob(IServiceScopeFactory scopes, ILogger<SmsDeliveryJob> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(30));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                log.LogInformation("SmsDeliveryJob: Process queued SMS delivery work.");
                using var scope = scopes.CreateScope();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "SmsDeliveryJob failed");
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