using EventBus.Core;

namespace Common.Core.Events;

/// <summary>
/// Event published when a grade is posted for a student.
/// </summary>
public record GradePostedEvent(int StudentId, string CourseName, double Grade, Guid TenantId)
    : IEvent;
