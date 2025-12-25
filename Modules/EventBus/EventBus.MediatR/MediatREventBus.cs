using EventBus.Core;
using MediatR;

namespace EventBus.MediatR;

public class MediatREventBus(IMediator mediator) : IEventBus
{
    public Task Dispatch(IEvent domainEvent, CancellationToken ct = default)
    {
        var eventToPublish = (INotification) Activator.CreateInstance(
            typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()),
            domainEvent
        )!;
        
        return mediator.Publish(eventToPublish, ct);
    }
}
