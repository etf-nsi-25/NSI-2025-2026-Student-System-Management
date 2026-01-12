using Common.Core.Tenant;
using EventBus.Core;
using MediatR;

namespace EventBus.MediatR;

/// <summary>
/// MediatR-based EventBus implementation. Handlers should be registered by implementing the
/// DomainEventNotification interface for a specific event.
/// </summary>
public class MediatREventBus(IMediator mediator, IScopedTenantContext tenantContext) : IEventBus
{
    public Task Dispatch(IEvent domainEvent, CancellationToken ct = default)
    {
        var eventToPublish = (INotification)Activator.CreateInstance(
            typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()),
            domainEvent
        )!;

        return mediator.Publish(eventToPublish, ct);
    }

    public Task Dispatch(IEvent domainEvent, Guid tenantId, CancellationToken ct = default)
    {
        using (tenantContext.Use(tenantId))
        {
            return Dispatch(domainEvent, ct);
        }
    }
}