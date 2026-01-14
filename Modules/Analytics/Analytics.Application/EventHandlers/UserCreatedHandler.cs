using Analytics.Application.Interfaces;
using Identity.Core.Events;
using EventBus.MediatR;
using MediatR;
using Analytics.Core.Entities;
using Analytics.Core.Constants;
using Identity.Core.Enums;

namespace Analytics.Application.EventHandler;

public class UserCreatedHandler(
    IStatsService statsService)
    : INotificationHandler<DomainEventNotification<UserCreatedEvent>>
{
    public async Task Handle(
        DomainEventNotification<UserCreatedEvent> notification,
        CancellationToken cancellationToken)
    {
        if (notification.Event.Role == UserRole.Student)
        {
            await statsService.GetOrUpdateStatAsync(MetricKey.CountStudents, Scope.University, Guid.Empty, true);
        }
        
    }
}
