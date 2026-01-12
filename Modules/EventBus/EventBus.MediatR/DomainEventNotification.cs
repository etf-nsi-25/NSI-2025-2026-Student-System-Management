using EventBus.Core;
using MediatR;

namespace EventBus.MediatR;

public class DomainEventNotification<T>(T domainEvent) : INotification where T : IEvent
{
    public T Event { get; } = domainEvent;
}
