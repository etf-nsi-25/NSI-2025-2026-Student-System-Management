using EventBus.MediatR;
using Faculty.Application.Services;
using Faculty.Infrastructure.Http;
using Identity.Core.Events;
using MediatR;

namespace Faculty.Infrastructure.EventHandler;

public class UserCreatedEventHandler(StudentService studentService, ITenantContext tenantContext)
    : INotificationHandler<DomainEventNotification<UserCreatedEvent>>
{
    public async Task Handle(
        DomainEventNotification<UserCreatedEvent> notification,
        CancellationToken cancellationToken)
    {
        using (tenantContext.Use(notification.Event.FacultyId))
        {
            await studentService.HandleStudentCreated(notification.Event, cancellationToken);
        }
    }
}
