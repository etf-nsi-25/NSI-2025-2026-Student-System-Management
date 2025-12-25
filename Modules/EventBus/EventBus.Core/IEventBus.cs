namespace EventBus.Core;

public interface IEventBus
{
    Task Dispatch(IEvent domainEvent, CancellationToken ct = default);
}
