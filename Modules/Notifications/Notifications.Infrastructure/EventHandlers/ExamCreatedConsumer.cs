using Common.Core.Events;
using EventBus.MediatR;
using MediatR;
using Notifications.Core.Interfaces;

namespace Notifications.Infrastructure.EventHandlers;

/// <summary>
/// Consumer for ExamCreatedEvent.
/// This handler listens for exam created events and triggers the notification service.
/// NO BUSINESS LOGIC - only extracts payload and calls the service.
/// </summary>
public class ExamCreatedConsumer : INotificationHandler<DomainEventNotification<ExamCreatedEvent>>
{
    private readonly INotificationService _notificationService;

    public ExamCreatedConsumer(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(
        DomainEventNotification<ExamCreatedEvent> notification,
        CancellationToken cancellationToken
    )
    {
        var eventData = notification.Event;

        // Extract payload and delegate to service
        await _notificationService.ProcessExamNotificationAsync(
            eventData.CourseId,
            eventData.ExamDate,
            eventData.TenantId,
            cancellationToken
        );
    }
}
