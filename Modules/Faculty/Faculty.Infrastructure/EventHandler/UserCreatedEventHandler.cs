using EventBus.MediatR;
using Faculty.Application.Services;
using Identity.Core.Events;
using MediatR;

namespace Faculty.Infrastructure.EventHandler;

public class UserCreatedEventHandler(StudentService studentService)
    : INotificationHandler<DomainEventNotification<UserCreatedEvent>>
{
    public async Task Handle(
        DomainEventNotification<UserCreatedEvent> notification,
        CancellationToken cancellationToken)
    {
        await studentService.HandleStudentCreated(notification.Event, cancellationToken);
    }
}