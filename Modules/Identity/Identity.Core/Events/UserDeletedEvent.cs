using EventBus.Core;

namespace Identity.Core.Events;

public record UserDeletedEvent(
    Guid UserId 
): IEvent;
