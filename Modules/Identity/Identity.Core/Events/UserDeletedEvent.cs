using EventBus.Core;

namespace Identity.Core.Events;

public record UserDeletedEvent(
    string UserId
): IEvent;
