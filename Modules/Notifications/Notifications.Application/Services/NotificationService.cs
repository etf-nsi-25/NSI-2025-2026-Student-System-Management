using Microsoft.Extensions.Logging;
using Notifications.Core.Entities;
using Notifications.Core.Interfaces;

namespace Notifications.Application.Services;

/// <summary>
/// Service implementation for processing and logging notifications.
/// Contains the business logic for handling different notification scenarios.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        ILogger<NotificationService> logger
    )
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task ProcessGradeNotificationAsync(
        int studentId,
        string courseName,
        double grade,
        Guid tenantId,
        CancellationToken cancellationToken = default
    )
    {
        if (studentId <= 0)
        {
            throw new ArgumentException("studentId must be a positive value.", nameof(studentId));
        }

        if (string.IsNullOrWhiteSpace(courseName))
        {
            throw new ArgumentException("courseName cannot be null or whitespace.", nameof(courseName));
        }

        try
        {
            // Resolve user destination (mocked for now - in real scenario, query Identity service)
            var userId = studentId.ToString();
            var destination = $"student{studentId}@unsa.ba"; // Mock email

            // Format notification message
            var message = $"You received a grade: {grade:F2} for {courseName}";

            // Create notification log entry
            var notificationLog = new NotificationLog
            {
                UserId = userId,
                ChannelType = "Email",
                Destination = destination,
                Message = message,
                SentAt = DateTime.UtcNow,
                Status = "Sent", // In production, this would be "Pending" until actual sending
            };

            // Persist notification
            await _notificationRepository.SaveAsync(notificationLog, cancellationToken);

            // Log for debugging/monitoring
            _logger.LogInformation(
                "Grade notification processed for StudentId={StudentId}, Course={CourseName}, Grade={Grade}, TenantId={TenantId}",
                studentId,
                courseName,
                grade,
                tenantId
            );

            // TODO: In production, call IEmailSender or queue for background processing
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process grade notification for StudentId={StudentId}, Course={CourseName}",
                studentId,
                courseName
            );
            throw;
        }
    }

    public async Task ProcessExamNotificationAsync(
        Guid courseId,
        DateTime examDate,
        Guid tenantId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            // In production: Query all enrolled students for this course
            // For now, we'll log a placeholder notification
            var destination = "students@unsa.ba"; // Mock - should be list of enrolled students
            var message =
                $"A new exam has been scheduled for course {courseId} on {examDate:yyyy-MM-dd HH:mm}. Please register before the deadline.";

            var notificationLog = new NotificationLog
            {
                UserId = "system", // Placeholder - in production, create one per enrolled student
                ChannelType = "Email",
                Destination = destination,
                Message = message,
                SentAt = DateTime.UtcNow,
                Status = "Sent",
            };

            await _notificationRepository.SaveAsync(notificationLog, cancellationToken);

            _logger.LogInformation(
                "Exam notification processed for CourseId={CourseId}, ExamDate={ExamDate}, TenantId={TenantId}",
                courseId,
                examDate,
                tenantId
            );

            // TODO: Query enrolled students and send individual notifications
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process exam notification for CourseId={CourseId}",
                courseId
            );
            throw;
        }
    }

    public async Task ProcessRequestApprovalNotificationAsync(
        int requestId,
        string requesterId,
        string requestType,
        string status,
        Guid tenantId,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(requestType))
        {
            throw new ArgumentException("requestType cannot be null or whitespace.", nameof(requestType));
        }

        try
        {
            // Resolve user destination (mocked for now)
            var destination = $"user{requesterId}@unsa.ba"; // Mock email

            // Format notification message based on status
            var message = status.ToLower() switch
            {
                "approved" => $"Your {requestType} request (ID: {requestId}) has been approved.",
                "rejected" => $"Your {requestType} request (ID: {requestId}) has been rejected.",
                "pending" => $"Your {requestType} request (ID: {requestId}) is pending approval.",
                _ =>
                    $"Your {requestType} request (ID: {requestId}) status has been updated to: {status}.",
            };

            var notificationLog = new NotificationLog
            {
                UserId = requesterId,
                ChannelType = "Email",
                Destination = destination,
                Message = message,
                SentAt = DateTime.UtcNow,
                Status = "Sent",
            };

            await _notificationRepository.SaveAsync(notificationLog, cancellationToken);

            _logger.LogInformation(
                "Request approval notification processed for RequestId={RequestId}, RequesterId={RequesterId}, Type={RequestType}, Status={Status}, TenantId={TenantId}",
                requestId,
                requesterId,
                requestType,
                status,
                tenantId
            );

            // TODO: Actual email sending via IEmailSender
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process request approval notification for RequestId={RequestId}, RequesterId={RequesterId}",
                requestId,
                requesterId
            );
            throw;
        }
    }
}
