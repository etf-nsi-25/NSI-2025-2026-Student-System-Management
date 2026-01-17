using Notifications.Core.Entities;

namespace Notifications.Application.Services;

public interface INotificationService
{
    public Task SendNotification(Notification notification);
}
