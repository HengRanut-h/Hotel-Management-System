using System.Text;
using HotelManagement.Application.Abstractions.Authentication;
using HotelManagement.Application.Abstractions.Caching;
using HotelManagement.Application.Abstractions.DateTime;
using HotelManagement.Application.Abstractions.Email;
using HotelManagement.Application.Abstractions.FileStorage;
using HotelManagement.Application.Abstractions.Payments;
using HotelManagement.Application.Abstractions.Notifications;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Abstractions.Sms;
using HotelManagement.Domain.Modules.Identity.Entities;
using HotelManagement.Infrastructure.Authentication;
using HotelManagement.Infrastructure.Authorization;
using HotelManagement.Infrastructure.Caching;
using HotelManagement.Infrastructure.DateTime;
using HotelManagement.Infrastructure.Email;
using HotelManagement.Infrastructure.FileStorage;
using HotelManagement.Infrastructure.Payments;
using HotelManagement.Infrastructure.Pdf;
using HotelManagement.Infrastructure.Notifications;
using HotelManagement.Infrastructure.Security;
using HotelManagement.Infrastructure.Sms;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HotelManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        var jwt = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
        if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
            throw new InvalidOperationException("Jwt:Key must contain at least 32 characters.");
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher<User>, HotelPasswordHasher>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddSingleton<ICacheService, MemoryCacheService>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IEmailSender, ConsoleEmailSender>();
        services.AddScoped<ISmsSender, ConsoleSmsSender>();
        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<IPaymentGateway, ManualPaymentGateway>();
        services.AddScoped<INotificationPublisher, NotificationPublisher>();
        services.AddScoped<InvoicePdfGenerator>();
        services.AddScoped<ReceiptPdfGenerator>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, ValidIssuer = jwt.Issuer, ValidateAudience = true,
                ValidAudience = jwt.Audience, ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                ValidateLifetime = true, ClockSkew = TimeSpan.FromSeconds(30)
            };
        });
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        return services;
    }
}