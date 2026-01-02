using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Application.Services;
using Notifications.Core.Interfaces;
using Notifications.Infrastructure.Db;
using Notifications.Infrastructure.EventHandlers;
using Notifications.Infrastructure.Repositories;

namespace Notifications.Infrastructure.DependencyInjection;

/// <summary>
/// Dependency injection configuration for the Notifications module.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationsModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Register repositories
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // Register services
        services.AddScoped<INotificationService, NotificationService>();

        // Register event handlers
        services.AddScoped<GradePostedConsumer>();
        services.AddScoped<ExamCreatedConsumer>();
        services.AddScoped<RequestApprovalConsumer>();

        // Register DbContext with SQL Server
        services.AddDbContext<NotificationsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Database"))
        );

        return services;
    }
}
