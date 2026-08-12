using HotelManagement.Worker.Jobs; using HotelManagement.Worker.Services;
namespace HotelManagement.Worker; public static class DependencyInjection { public static IServiceCollection AddWorkerServices(this IServiceCollection services){services.AddSingleton<BackgroundJobState>();services.AddHostedService<ReservationReminderJob>();
        services.AddHostedService<NoShowProcessingJob>();
        services.AddHostedService<InvoiceOverdueJob>();
        services.AddHostedService<UtilityBillingJob>();
        services.AddHostedService<EmailDeliveryJob>();
        services.AddHostedService<SmsDeliveryJob>();
        services.AddHostedService<DailyRevenueJob>();
        services.AddHostedService<BackupJob>();return services;} }
