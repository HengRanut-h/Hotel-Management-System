using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Domain.Modules.Identity.Entities;
using HotelManagement.Persistence.Context;
using HotelManagement.Persistence.Interceptors;
using HotelManagement.Persistence.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection is missing.");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection is empty.");
        }

        var configuredServerVersion =
            configuration["Database:ServerVersion"];

        var serverVersion =
            string.IsNullOrWhiteSpace(configuredServerVersion)
                ? ServerVersion.AutoDetect(connectionString)
                : ServerVersion.Parse(configuredServerVersion);

        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<DomainEventInterceptor>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseMySql(
                connectionString,
                serverVersion);

            options.AddInterceptors(
                serviceProvider.GetRequiredService<AuditableEntityInterceptor>(),
                serviceProvider.GetRequiredService<SoftDeleteInterceptor>(),
                serviceProvider.GetRequiredService<DomainEventInterceptor>());
        });

        services.AddScoped<IApplicationDbContext>(
            sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    public static async Task InitializeDatabaseAsync(
        this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger =
            scope.ServiceProvider
                .GetRequiredService<ILogger<ApplicationDbContext>>();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var migrations = db.Database.GetMigrations().ToList();
        if (migrations.Count == 0)
            return;

        var applyMigrations = app.Configuration.GetValue(
            "Database:ApplyMigrationsOnStartup",
            app.Environment.IsDevelopment());

        var pendingMigrations =
            (await db.Database.GetPendingMigrationsAsync()).ToList();

        if (applyMigrations)
        {
            await db.Database.MigrateAsync();
        }
        else if (pendingMigrations.Count > 0)
        {
            logger.LogWarning(
                "Database has {PendingMigrationCount} pending EF Core migrations. Skipping seed data until migrations are applied.",
                pendingMigrations.Count);

            return;
        }

        if (!await db.Database.CanConnectAsync())
            return;

        var passwordHasher =
            scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        await DatabaseSeeder.SeedAsync(
            db,
            app.Configuration,
            passwordHasher);
    }
}
