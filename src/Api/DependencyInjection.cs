using HotelManagement.Api.HealthChecks;
using HotelManagement.Api.Hubs;
using HotelManagement.Api.Middleware;
using HotelManagement.Api.OpenApi;
using HotelManagement.Api.RateLimiting;

namespace HotelManagement.Api;

public static class DependencyInjection
{
    // =========================================================
    // ADD API SERVICES
    // =========================================================

    public static IServiceCollection AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // =====================================================
        // CONTROLLERS
        // =====================================================

        services.AddControllers();

        // =====================================================
        // SWAGGER / OPENAPI
        // =====================================================

        services.AddSwaggerConfiguration();

        // =====================================================
        // CORS
        // =====================================================

        var allowedOrigins =
            configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>()
            ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(
                "Angular",
                policy =>
                {
                    if (allowedOrigins.Length > 0)
                    {
                        policy
                            .WithOrigins(allowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }
                    else
                    {
                        policy
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(_ => true)
                            .AllowCredentials();
                    }
                });
        });

        // =====================================================
        // HEALTH CHECKS
        // =====================================================

        services
            .AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>(
                "database");

        // =====================================================
        // RATE LIMITING
        // =====================================================

        services.AddApiRateLimiting();

        // =====================================================
        // SIGNALR
        // =====================================================

        services.AddSignalR();

        return services;
    }

    // =========================================================
    // USE API PIPELINE
    // =========================================================

    public static WebApplication UseApi(
        this WebApplication app)
    {
        // =====================================================
        // EXCEPTION HANDLING
        // =====================================================

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // =====================================================
        // CORRELATION ID
        // =====================================================

        app.UseMiddleware<CorrelationIdMiddleware>();

        // =====================================================
        // REQUEST LOGGING
        // =====================================================

        app.UseMiddleware<RequestLoggingMiddleware>();

        // =====================================================
        // SECURITY HEADERS
        // =====================================================

        app.UseMiddleware<SecurityHeadersMiddleware>();

        // =====================================================
        // SWAGGER
        // =====================================================

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "Hotel Management API v1");

                options.DocumentTitle =
                    "Hotel Management API";
            });
        }

        // =====================================================
        // HTTPS REDIRECTION
        // =====================================================

        app.UseHttpsRedirection();

        // =====================================================
        // CORS
        // =====================================================

        app.UseCors("Angular");

        // =====================================================
        // RATE LIMITING
        // =====================================================

        app.UseRateLimiter();

        // =====================================================
        // AUTHENTICATION
        // =====================================================

        app.UseAuthentication();

        // =====================================================
        // HOTEL CONTEXT
        // =====================================================

        app.UseMiddleware<HotelContextMiddleware>();

        // =====================================================
        // BRANCH CONTEXT
        // =====================================================

        app.UseMiddleware<BranchContextMiddleware>();

        // =====================================================
        // AUTHORIZATION
        // =====================================================

        app.UseAuthorization();

        // =====================================================
        // CONTROLLERS
        // =====================================================

        app.MapControllers();

        // =====================================================
        // HEALTH CHECK
        // GET /health
        // =====================================================

        app.MapHealthChecks("/health");

        // =====================================================
        // NOTIFICATION HUB
        // /hubs/notifications
        // =====================================================

        app.MapHub<NotificationHub>(
            "/hubs/notifications");

        // =====================================================
        // FRONT DESK HUB
        // /hubs/front-desk
        // =====================================================

        app.MapHub<FrontDeskHub>(
            "/hubs/front-desk");

        // =====================================================
        // HOUSEKEEPING HUB
        // /hubs/housekeeping
        // =====================================================

        app.MapHub<HousekeepingHub>(
            "/hubs/housekeeping");

        // =====================================================
        // MAINTENANCE HUB
        // /hubs/maintenance
        // =====================================================

        app.MapHub<MaintenanceHub>(
            "/hubs/maintenance");

        return app;
    }
}