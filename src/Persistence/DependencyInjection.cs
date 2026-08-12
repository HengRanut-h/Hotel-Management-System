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

        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<DomainEventInterceptor>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString));

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
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var migrations = db.Database.GetMigrations().ToList();
        if (migrations.Count == 0)
            return;

        var applyMigrations = app.Configuration.GetValue(
            "Database:ApplyMigrationsOnStartup",
            app.Environment.IsDevelopment());

        if (applyMigrations)
            await db.Database.MigrateAsync();

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
