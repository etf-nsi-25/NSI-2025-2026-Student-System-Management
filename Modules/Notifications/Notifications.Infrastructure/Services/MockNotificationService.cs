using Microsoft.Extensions.Logging;
using Notifications.Application.Services;
using Notifications.Core.Entities;

namespace Notifications.Infrastructure.Services;

public class MockNotificationService(ILogger<MockNotificationService> logger) : INotificationService
{
    public Task SendNotification(Notification notification)
    {
        logger.LogInformation(
            "Mocking notification call. \n {Recipient} \n {Title} \n {Body}",
            notification.Recipients,
            notification.Title,
            notification.Body
        );
        
        return Task.CompletedTask;
    }
}
