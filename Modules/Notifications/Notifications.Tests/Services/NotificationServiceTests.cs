using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Notifications.Application.Services;
using Notifications.Core.Entities;
using Notifications.Core.Interfaces;

namespace Notifications.Tests.Services;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _mockRepository;
    private readonly Mock<ILogger<NotificationService>> _mockLogger;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        _mockRepository = new Mock<INotificationRepository>();
        _mockLogger = new Mock<ILogger<NotificationService>>();
        _service = new NotificationService(_mockRepository.Object, _mockLogger.Object);
    }

    #region Grade Notification Tests

    [Fact]
    public async Task ProcessGradeNotificationAsync_WithValidData_SavesNotificationLog()
    {
        // Arrange
        var studentId = 123;
        var courseName = "Mathematics";
        var grade = 8.5;
        var tenantId = Guid.NewGuid();
        
        var savedNotification = new NotificationLog();
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationLog n, CancellationToken ct) => n)
            .Callback<NotificationLog, CancellationToken>((n, _) =>
            {
                savedNotification = n;
            });

        // Act
        await _service.ProcessGradeNotificationAsync(studentId, courseName, grade, tenantId);

        // Assert
        _mockRepository.Verify(
            r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()),
            Times.Once);

        savedNotification.UserId.Should().Be(studentId.ToString());
        savedNotification.ChannelType.Should().Be("Email");
        savedNotification.Message.Should().Contain("Mathematics");
        savedNotification.Message.Should().Contain("8.5");
        savedNotification.Status.Should().Be("Sent");
    }

    [Fact]
    public async Task ProcessGradeNotificationAsync_WithDifferentGrades_FormatsMessageCorrectly()
    {
        // Arrange
        var grades = new[] { 5.0, 7.5, 9.5, 10.0 };
        var savedNotifications = new List<NotificationLog>();

        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationLog n, CancellationToken ct) => n)
            .Callback<NotificationLog, CancellationToken>((n, _) => savedNotifications.Add(n));

        // Act
        foreach (var grade in grades)
        {
            await _service.ProcessGradeNotificationAsync(1, "Physics", grade, Guid.NewGuid());
        }

        // Assert
        savedNotifications.Should().HaveCount(4);
        savedNotifications[0].Message.Should().Contain("5.00");
        savedNotifications[1].Message.Should().Contain("7.50");
        savedNotifications[2].Message.Should().Contain("9.50");
        savedNotifications[3].Message.Should().Contain("10.00");
    }

    [Fact]
    public async Task ProcessGradeNotificationAsync_SetsCorrectDestination()
    {
        // Arrange
        var studentId = 456;
        var savedNotification = new NotificationLog();
        
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationLog n, CancellationToken ct) => n)
            .Callback<NotificationLog, CancellationToken>((n, _) => savedNotification = n);

        // Act
        await _service.ProcessGradeNotificationAsync(studentId, "English", 7.0, Guid.NewGuid());

        // Assert
        savedNotification.Destination.Should().Contain("student456");
        savedNotification.Destination.Should().EndWith("@unsa.ba");
    }

    [Fact]
    public async Task ProcessGradeNotificationAsync_SetsSentAtToUtcNow()
    {
        // Arrange
        var beforeExecution = DateTime.UtcNow;
        var savedNotification = new NotificationLog();
        
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationLog n, CancellationToken ct) => n)
            .Callback<NotificationLog, CancellationToken>((n, _) => savedNotification = n);

        // Act
        await _service.ProcessGradeNotificationAsync(1, "Chemistry", 6.5, Guid.NewGuid());
        var afterExecution = DateTime.UtcNow;

        // Assert
        savedNotification.SentAt.Should().BeOnOrAfter(beforeExecution);
        savedNotification.SentAt.Should().BeOnOrBefore(afterExecution.AddSeconds(1));
    }

    [Fact]
    public async Task ProcessGradeNotificationAsync_WhenRepositoryThrows_PropagatesException()
    {
        // Arrange
        var exception = new InvalidOperationException("Database error");
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.ProcessGradeNotificationAsync(1, "Math", 8.0, Guid.NewGuid()));
    }

    #endregion

    #region Exam Notification Tests

    [Fact]
    public async Task ProcessExamNotificationAsync_WithValidData_SavesNotificationLog()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var examDate = DateTime.UtcNow.AddDays(7);
        var tenantId = Guid.NewGuid();
        
        var savedNotification = new NotificationLog();
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationLog n, CancellationToken ct) => n)
            .Callback<NotificationLog, CancellationToken>((n, _) => savedNotification = n);

        // Act
        await _service.ProcessExamNotificationAsync(courseId, examDate, tenantId);

        // Assert
        _mockRepository.Verify(
            r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()),
            Times.Once);

        savedNotification.ChannelType.Should().Be("Email");
        savedNotification.Message.Should().Contain(courseId.ToString());
        savedNotification.Message.Should().Contain(examDate.ToString("yyyy-MM-dd HH:mm"));
        savedNotification.Status.Should().Be("Sent");
    }

    [Fact]
    public async Task ProcessExamNotificationAsync_WithDifferentExamDates_FormatsDateCorrectly()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(2026, 01, 15, 10, 30, 0, DateTimeKind.Utc),
            new DateTime(2026, 02, 20, 14, 45, 0, DateTimeKind.Utc),
            new DateTime(2026, 06, 10, 09, 00, 0, DateTimeKind.Utc)
        };

        var savedNotifications = new List<NotificationLog>();
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationLog n, CancellationToken ct) => n)
            .Callback<NotificationLog, CancellationToken>((n, _) => savedNotifications.Add(n));

        // Act
        foreach (var date in dates)
        {
            await _service.ProcessExamNotificationAsync(Guid.NewGuid(), date, Guid.NewGuid());
        }

        // Assert
        savedNotifications.Should().HaveCount(3);
        savedNotifications[0].Message.Should().Contain("2026-01-15 10:30");
        savedNotifications[1].Message.Should().Contain("2026-02-20 14:45");
        savedNotifications[2].Message.Should().Contain("2026-06-10 09:00");
    }

    [Fact]
    public async Task ProcessExamNotificationAsync_ContainsRegistrationDeadlineWarning()
    {
        // Arrange
        var savedNotification = new NotificationLog();
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationLog n, CancellationToken ct) => n)
            .Callback<NotificationLog, CancellationToken>((n, _) => savedNotification = n);

        // Act
        await _service.ProcessExamNotificationAsync(Guid.NewGuid(), DateTime.UtcNow.AddDays(5), Guid.NewGuid());

        // Assert
        savedNotification.Message.Should().Contain("register");
        savedNotification.Message.Should().Contain("deadline");
    }

    #endregion

    #region Request Approval Notification Tests

    [Fact]
    public async Task ProcessRequestApprovalNotificationAsync_WithApprovedStatus_SendsApprovalMessage()
    {
        // Arrange
        var requestId = 789;
        var requesterId = "user123";
        var requestType = "Transcript";
        var tenantId = Guid.NewGuid();
        
        var savedNotification = new NotificationLog();
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationLog n, CancellationToken ct) => n)
            .Callback<NotificationLog, CancellationToken>((n, _) => savedNotification = n);

        // Act
        await _service.ProcessRequestApprovalNotificationAsync(
            requestId, requesterId, requestType, "approved", tenantId);

        // Assert
        savedNotification.UserId.Should().Be(requesterId);
        savedNotification.Message.Should().Contain("approved");
        savedNotification.Message.Should().Contain(requestType);
        savedNotification.Message.Should().Contain(requestId.ToString());
    }

    [Fact]
    public async Task ProcessRequestApprovalNotificationAsync_WithRejectedStatus_SendsRejectionMessage()
    {
        // Arrange
        var savedNotification = new NotificationLog();
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationLog n, CancellationToken ct) => n)
            .Callback<NotificationLog, CancellationToken>((n, _) => savedNotification = n);

        // Act
        await _service.ProcessRequestApprovalNotificationAsync(
            100, "user456", "Certificate", "rejected", Guid.NewGuid());

        // Assert
        savedNotification.Message.Should().Contain("rejected");
    }

    [Fact]
    public async Task ProcessRequestApprovalNotificationAsync_WithPendingStatus_SendsPendingMessage()
    {
        // Arrange
        var savedNotification = new NotificationLog();
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationLog n, CancellationToken ct) => n)
            .Callback<NotificationLog, CancellationToken>((n, _) => savedNotification = n);

        // Act
        await _service.ProcessRequestApprovalNotificationAsync(
            101, "user789", "Enrollment", "pending", Guid.NewGuid());

        // Assert
        savedNotification.Message.Should().Contain("pending");
    }

    [Fact]
    public async Task ProcessRequestApprovalNotificationAsync_WithCustomStatus_SendsGenericMessage()
    {
        // Arrange
        var savedNotification = new NotificationLog();
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationLog n, CancellationToken ct) => n)
            .Callback<NotificationLog, CancellationToken>((n, _) => savedNotification = n);

        // Act
        await _service.ProcessRequestApprovalNotificationAsync(
            102, "user000", "Other", "in_review", Guid.NewGuid());

        // Assert
        savedNotification.Message.Should().Contain("status has been updated to");
        savedNotification.Message.Should().Contain("in_review");
    }

    [Fact]
    public async Task ProcessRequestApprovalNotificationAsync_SetsCorrectDestination()
    {
        // Arrange
        var requesterId = "john.doe";
        var savedNotification = new NotificationLog();
        
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationLog n, CancellationToken ct) => n)
            .Callback<NotificationLog, CancellationToken>((n, _) => savedNotification = n);

        // Act
        await _service.ProcessRequestApprovalNotificationAsync(
            50, requesterId, "Transcript", "approved", Guid.NewGuid());

        // Assert
        savedNotification.Destination.Should().Contain(requesterId);
        savedNotification.Destination.Should().EndWith("@unsa.ba");
    }

    #endregion

    #region Logging Tests

    [Fact]
    public async Task ProcessGradeNotificationAsync_LogsSuccessfulProcessing()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationLog n, CancellationToken ct) => n);

        // Act
        await _service.ProcessGradeNotificationAsync(1, "Math", 8.0, Guid.NewGuid());

        // Assert
        _mockLogger.VerifyLog(LogLevel.Information, "Grade notification processed");
    }

    [Fact]
    public async Task ProcessGradeNotificationAsync_LogsExceptionOnFailure()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            async () => await _service.ProcessGradeNotificationAsync(1, "Math", 8.0, Guid.NewGuid()));

        _mockLogger.VerifyLog(LogLevel.Error, "Failed to process grade notification");
    }

    #endregion
}

/// <summary>
/// Helper extension for verifying logger calls
/// </summary>
internal static class LoggerExtensions
{
    public static void VerifyLog(this Mock<ILogger<NotificationService>> logger, LogLevel level, string message)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
