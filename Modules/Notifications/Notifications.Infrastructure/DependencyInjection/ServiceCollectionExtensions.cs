using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Application.Services;
using Notifications.Infrastructure.Services;
using Notifications.Infrastructure.Settings;

namespace Notifications.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNotificationsModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<NotificationSettings>(configuration.GetSection("NotificationSettings"));

            var notificationSettings = new NotificationSettings();
            configuration.GetSection("NotificationSettings").Bind(notificationSettings);

            if (NotificationSettings.NotificationEngine.Mock == notificationSettings.NotificationEngineType)
            {
                services.AddScoped<INotificationService, MockNotificationService>();
            }
            else
            {
                services.AddScoped<INotificationService, MailNotificationService>();
            }

            return services;
        }
    }
}
