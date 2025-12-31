namespace EventBus.Core;

/// <summary>
/// Contract that describes ways to publish events.
/// <p>
/// Events can be listened to by registering event handlers in accordance to the implementation used at runtime. Events
/// can be listened to by any number of listeners. There is no guarantee in which order the listeners will be invoked
/// so they must not rely on each other.
/// </p>
/// <p>
/// Do note that the implementations of this interface do not
/// necessarily allow publishing events in a "fire-and-forget" fashion, so the methods of the interface
/// should always be awaited.
/// </p>
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an event.
    /// </summary>
    /// <param name="domainEvent">event to publish</param>
    /// <param name="ct">cancellation token</param>
    /// <returns></returns>
    Task Dispatch(IEvent domainEvent, CancellationToken ct = default);

    /// <summary>
    /// Publishes a tenant-aware event. This method should only be used from contexts where tenancy information
    /// is not otherwise available (such as from http context).
    /// </summary>
    /// <param name="domainEvent">event to publish</param>
    /// <param name="tenantId">tenant id</param>
    /// <param name="ct">cancellation token</param>
    /// <returns></returns>
    Task Dispatch(IEvent domainEvent, Guid tenantId, CancellationToken ct = default);
}