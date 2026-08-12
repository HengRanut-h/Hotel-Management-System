using System.Text.Json;
using HotelManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Worker.Jobs;

public sealed class BackupJob(IServiceScopeFactory scopes, IWebHostEnvironment env, ILogger<BackupJob> log)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(24));
        do
        {
            try
            {
                using var scope = scopes.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var snapshot = new
                {
                    createdAtUtc = DateTimeOffset.UtcNow, hotels = await db.Hotels.CountAsync(token),
                    rooms = await db.Rooms.CountAsync(token), guests = await db.Guests.CountAsync(token),
                    reservations = await db.Reservations.CountAsync(token),
                    invoices = await db.Invoices.CountAsync(token), payments = await db.Payments.CountAsync(token)
                };
                var dir = Path.Combine(env.ContentRootPath, "App_Data", "backups");
                Directory.CreateDirectory(dir);
                await File.WriteAllTextAsync(Path.Combine(dir, $"snapshot-{DateTime.UtcNow:yyyyMMdd}.json"),
                    JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true }), token);
                log.LogInformation(
                    "Backup snapshot marker created. Use MySQL-native backups for full database recovery.");
            }
            catch (Exception ex)when (ex is not OperationCanceledException)
            {
                log.LogError(ex, "BackupJob failed");
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