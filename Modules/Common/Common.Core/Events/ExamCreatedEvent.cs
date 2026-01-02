using EventBus.Core;

namespace Common.Core.Events;

/// <summary>
/// Event published when a new exam is created.
/// </summary>
public record ExamCreatedEvent(Guid CourseId, DateTime ExamDate, Guid TenantId) : IEvent;
