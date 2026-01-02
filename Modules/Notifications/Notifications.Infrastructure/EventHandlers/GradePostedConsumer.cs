using Common.Core.Events;
using EventBus.MediatR;
using MediatR;
using Notifications.Core.Interfaces;

namespace Notifications.Infrastructure.EventHandlers;

/// <summary>
/// Consumer for GradePostedEvent.
/// This handler listens for grade posted events and triggers the notification service.
/// NO BUSINESS LOGIC - only extracts payload and calls the service.
/// </summary>
public class GradePostedConsumer : INotificationHandler<DomainEventNotification<GradePostedEvent>>
{
    private readonly INotificationService _notificationService;

    public GradePostedConsumer(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(
        DomainEventNotification<GradePostedEvent> notification,
        CancellationToken cancellationToken)
    {
        var eventData = notification.Event;

        // Extract payload and delegate to service
        await _notificationService.ProcessGradeNotificationAsync(
            eventData.StudentId,
            eventData.CourseName,
            eventData.Grade,
            eventData.TenantId,
            cancellationToken);
    }
}
