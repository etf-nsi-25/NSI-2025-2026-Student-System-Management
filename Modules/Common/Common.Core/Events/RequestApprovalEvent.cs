using EventBus.Core;

namespace Common.Core.Events;

/// <summary>
/// Event published when a request requires approval or has been approved/rejected.
/// </summary>
public record RequestApprovalEvent(
    int RequestId,
    string RequesterId,
    string RequestType,
    string Status,
    Guid TenantId
) : IEvent;
