using Common.Core.Events;
using EventBus.MediatR;
using MediatR;
using Notifications.Core.Interfaces;

namespace Notifications.Infrastructure.EventHandlers;

/// <summary>
/// Consumer for RequestApprovalEvent.
/// This handler listens for request approval events and triggers the notification service.
/// NO BUSINESS LOGIC - only extracts payload and calls the service.
/// </summary>
public class RequestApprovalConsumer : INotificationHandler<DomainEventNotification<RequestApprovalEvent>>
{
    private readonly INotificationService _notificationService;

    public RequestApprovalConsumer(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(
        DomainEventNotification<RequestApprovalEvent> notification,
        CancellationToken cancellationToken)
    {
        var eventData = notification.Event;

        // Extract payload and delegate to service
        await _notificationService.ProcessRequestApprovalNotificationAsync(
            eventData.RequestId,
            eventData.RequesterId,
            eventData.RequestType,
            eventData.Status,
            eventData.TenantId,
            cancellationToken);
    }
}
