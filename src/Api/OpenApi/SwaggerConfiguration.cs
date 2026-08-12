using Microsoft.OpenApi.Models;

namespace HotelManagement.Api.OpenApi;

public static class SwaggerConfiguration
{
    public static IServiceCollection
        AddSwaggerConfiguration(
            this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title =
                        "Hotel Management API",
                    Version =
                        "v1",
                    Description =
                        "ASP.NET Core .NET 8 Hotel Management System API"
                });

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name =
                        "Authorization",
                    Type =
                        SecuritySchemeType.Http,
                    Scheme =
                        "bearer",
                    BearerFormat =
                        "JWT",
                    In =
                        ParameterLocation.Header,
                    Description =
                        "Enter your JWT bearer token."
                });

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference =
                                new OpenApiReference
                                {
                                    Type =
                                        ReferenceType.SecurityScheme,
                                    Id =
                                        "Bearer"
                                }
                        },
                        Array.Empty<string>()
                    }
                });
        });

        return services;
    }
}