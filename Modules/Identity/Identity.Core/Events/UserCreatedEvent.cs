using EventBus.Core;
using Identity.Core.Enums;

namespace Identity.Core.Events;

public record UserCreatedEvent(
    Guid UserId, 
    string Username, 
    string FirstName, 
    string LastName, 
    Guid FacultyId, 
    UserRole Role,
    string? IndexNumber
) : IEvent;
