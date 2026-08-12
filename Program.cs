using System.Text.Json.Serialization;
using HotelManagement.Api;
using HotelManagement.Application;
using HotelManagement.Infrastructure;
using HotelManagement.Persistence;
using HotelManagement.Worker;

var builder =
    WebApplication.CreateBuilder(args);

// =========================================================
// CONTROLLERS + JSON OPTIONS
// =========================================================

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull;
    });

// =========================================================
// APPLICATION SERVICES
// =========================================================

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddApi(builder.Configuration)
    .AddWorkerServices();

// =========================================================
// BUILD APPLICATION
// =========================================================

var app =
    builder.Build();

// =========================================================
// INITIALIZE DATABASE
// =========================================================

await app.InitializeDatabaseAsync();

// =========================================================
// API PIPELINE
// =========================================================

app.UseApi();

// =========================================================
// RUN APPLICATION
// =========================================================

app.Run();

public partial class Program;