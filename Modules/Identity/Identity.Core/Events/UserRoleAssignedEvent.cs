using System;
using Identity.Core.Enums;

namespace Identity.Core.Events;


public record UserRoleAssignedEvent(
    Guid UserId, 
    UserRole PreviousRole, 
    UserRole NewRole
);