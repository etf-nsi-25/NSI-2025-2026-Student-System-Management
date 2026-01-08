namespace Notifications.Core.Interfaces;

/// <summary>
/// Service interface for processing notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Processes a grade posted notification.
    /// </summary>
    /// <param name="studentId">The student's ID.</param>
    /// <param name="courseName">The name of the course.</param>
    /// <param name="grade">The grade received.</param>
    /// <param name="tenantId">The tenant (faculty) ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ProcessGradeNotificationAsync(int studentId, string courseName, double grade, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes an exam created notification.
    /// </summary>
    /// <param name="courseId">The course ID.</param>
    /// <param name="examDate">The exam date.</param>
    /// <param name="tenantId">The tenant (faculty) ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ProcessExamNotificationAsync(Guid courseId, DateTime examDate, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes a request approval notification.
    /// </summary>
    /// <param name="requestId">The request ID.</param>
    /// <param name="requesterId">The requester's user ID.</param>
    /// <param name="requestType">The type of request.</param>
    /// <param name="status">The status of the request.</param>
    /// <param name="tenantId">The tenant (faculty) ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ProcessRequestApprovalNotificationAsync(int requestId, string requesterId, string requestType, string status, Guid tenantId, CancellationToken cancellationToken = default);
}
