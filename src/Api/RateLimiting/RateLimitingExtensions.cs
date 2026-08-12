using System.Security.Claims;
using System.Threading.RateLimiting;

using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Api.RateLimiting;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddApiRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // =====================================================
            // GLOBAL LIMITER
            // =====================================================

            options.GlobalLimiter =
                PartitionedRateLimiter.Create<HttpContext, string>(
                    httpContext =>
                    {
                        var userId =
                            httpContext.User.Identity?.IsAuthenticated == true
                                ? httpContext.User.FindFirstValue(
                                    ClaimTypes.NameIdentifier)
                                : null;

                        var ipAddress =
                            httpContext.Connection.RemoteIpAddress?.ToString();

                        var partitionKey =
                            userId
                            ?? ipAddress
                            ?? "anonymous";

                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey,
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 300,
                                Window = TimeSpan.FromMinutes(1),
                                QueueLimit = 20,
                                QueueProcessingOrder =
                                    QueueProcessingOrder.OldestFirst,
                                AutoReplenishment = true
                            });
                    });

            // =====================================================
            // AUTH
            // =====================================================

            options.AddSlidingWindowLimiter(
                RateLimitPolicies.Auth,
                limiterOptions =>
                {
                    limiterOptions.PermitLimit = 10;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.SegmentsPerWindow = 6;

                    limiterOptions.QueueLimit = 0;

                    limiterOptions.QueueProcessingOrder =
                        QueueProcessingOrder.OldestFirst;

                    limiterOptions.AutoReplenishment = true;
                });

            // =====================================================
            // NORMAL API
            // =====================================================

            options.AddTokenBucketLimiter(
                RateLimitPolicies.Api,
                limiterOptions =>
                {
                    limiterOptions.TokenLimit = 120;
                    limiterOptions.TokensPerPeriod = 120;

                    limiterOptions.ReplenishmentPeriod =
                        TimeSpan.FromMinutes(1);

                    limiterOptions.QueueLimit = 20;

                    limiterOptions.QueueProcessingOrder =
                        QueueProcessingOrder.OldestFirst;

                    limiterOptions.AutoReplenishment = true;
                });

            // =====================================================
            // WRITE
            // =====================================================

            options.AddSlidingWindowLimiter(
                RateLimitPolicies.Write,
                limiterOptions =>
                {
                    limiterOptions.PermitLimit = 60;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.SegmentsPerWindow = 6;

                    limiterOptions.QueueLimit = 10;

                    limiterOptions.QueueProcessingOrder =
                        QueueProcessingOrder.OldestFirst;

                    limiterOptions.AutoReplenishment = true;
                });

            // =====================================================
            // EXPENSIVE
            // =====================================================

            options.AddFixedWindowLimiter(
                RateLimitPolicies.Expensive,
                limiterOptions =>
                {
                    limiterOptions.PermitLimit = 10;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);

                    limiterOptions.QueueLimit = 2;

                    limiterOptions.QueueProcessingOrder =
                        QueueProcessingOrder.OldestFirst;

                    limiterOptions.AutoReplenishment = true;
                });

            // =====================================================
            // UPLOAD
            // =====================================================

            options.AddConcurrencyLimiter(
                RateLimitPolicies.Upload,
                limiterOptions =>
                {
                    limiterOptions.PermitLimit = 3;
                    limiterOptions.QueueLimit = 5;

                    limiterOptions.QueueProcessingOrder =
                        QueueProcessingOrder.OldestFirst;
                });

            // =====================================================
            // REJECTION
            // =====================================================

            options.RejectionStatusCode =
                StatusCodes.Status429TooManyRequests;

            options.OnRejected =
                async (
                    context,
                    cancellationToken) =>
                {
                    var httpContext =
                        context.HttpContext;

                    if (
                        context.Lease.TryGetMetadata(
                            MetadataName.RetryAfter,
                            out var retryAfter))
                    {
                        httpContext.Response.Headers["Retry-After"] =
                            Math.Ceiling(
                                retryAfter.TotalSeconds)
                            .ToString();
                    }

                    var response =
                        ApiErrorResponse.Create(
                            StatusCodes.Status429TooManyRequests,
                            "RATE_LIMIT_EXCEEDED",
                            "Too many requests. Please try again later.",
                            traceId:
                                httpContext.TraceIdentifier);

                    await httpContext.Response.WriteAsJsonAsync(
                        response,
                        cancellationToken:
                            cancellationToken);
                };
        });

        return services;
    }
}