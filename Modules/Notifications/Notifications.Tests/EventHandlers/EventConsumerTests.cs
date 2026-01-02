using Common.Core.Events;
using EventBus.MediatR;
using FluentAssertions;
using Moq;
using Notifications.Core.Interfaces;
using Notifications.Infrastructure.EventHandlers;

namespace Notifications.Tests.EventHandlers;

public class GradePostedConsumerTests
{
    private readonly Mock<INotificationService> _mockService;
    private readonly GradePostedConsumer _consumer;

    public GradePostedConsumerTests()
    {
        _mockService = new Mock<INotificationService>();
        _consumer = new GradePostedConsumer(_mockService.Object);
    }

    [Fact]
    public async Task Handle_WithValidGradePostedEvent_CallsNotificationService()
    {
        // Arrange
        var gradeEvent = new GradePostedEvent(
            StudentId: 123,
            CourseName: "Mathematics",
            Grade: 8.5,
            TenantId: Guid.NewGuid()
        );

        var notification = new DomainEventNotification<GradePostedEvent>(gradeEvent);

        // Act
        await _consumer.Handle(notification, CancellationToken.None);

        // Assert
        _mockService.Verify(
            s =>
                s.ProcessGradeNotificationAsync(
                    123,
                    "Mathematics",
                    8.5,
                    gradeEvent.TenantId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ExtractsAllEventProperties()
    {
        // Arrange
        var studentId = 456;
        var courseName = "Physics";
        var grade = 9.5;
        var tenantId = Guid.NewGuid();

        var gradeEvent = new GradePostedEvent(studentId, courseName, grade, tenantId);
        var notification = new DomainEventNotification<GradePostedEvent>(gradeEvent);

        // Act
        await _consumer.Handle(notification, CancellationToken.None);

        // Assert
        _mockService.Verify(
            s =>
                s.ProcessGradeNotificationAsync(
                    studentId,
                    courseName,
                    grade,
                    tenantId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_PassesCancellationToken()
    {
        // Arrange
        var gradeEvent = new GradePostedEvent(1, "Chemistry", 7.0, Guid.NewGuid());
        var notification = new DomainEventNotification<GradePostedEvent>(gradeEvent);
        var cts = new CancellationTokenSource();

        // Act
        await _consumer.Handle(notification, cts.Token);

        // Assert
        _mockService.Verify(
            s =>
                s.ProcessGradeNotificationAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<Guid>(),
                    cts.Token
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenServiceThrows_PropagatesException()
    {
        // Arrange
        var gradeEvent = new GradePostedEvent(1, "Math", 8.0, Guid.NewGuid());
        var notification = new DomainEventNotification<GradePostedEvent>(gradeEvent);
        var exception = new InvalidOperationException("Service error");

        _mockService
            .Setup(s =>
                s.ProcessGradeNotificationAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _consumer.Handle(notification, CancellationToken.None)
        );
    }
}

public class ExamCreatedConsumerTests
{
    private readonly Mock<INotificationService> _mockService;
    private readonly ExamCreatedConsumer _consumer;

    public ExamCreatedConsumerTests()
    {
        _mockService = new Mock<INotificationService>();
        _consumer = new ExamCreatedConsumer(_mockService.Object);
    }

    [Fact]
    public async Task Handle_WithValidExamCreatedEvent_CallsNotificationService()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var examDate = DateTime.UtcNow.AddDays(7);
        var tenantId = Guid.NewGuid();

        var examEvent = new ExamCreatedEvent(courseId, examDate, tenantId);
        var notification = new DomainEventNotification<ExamCreatedEvent>(examEvent);

        // Act
        await _consumer.Handle(notification, CancellationToken.None);

        // Assert
        _mockService.Verify(
            s =>
                s.ProcessExamNotificationAsync(
                    courseId,
                    examDate,
                    tenantId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ExtractsAllEventProperties()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var examDate = new DateTime(2026, 3, 15, 10, 30, 0, DateTimeKind.Utc);
        var tenantId = Guid.NewGuid();

        var examEvent = new ExamCreatedEvent(courseId, examDate, tenantId);
        var notification = new DomainEventNotification<ExamCreatedEvent>(examEvent);

        // Act
        await _consumer.Handle(notification, CancellationToken.None);

        // Assert
        _mockService.Verify(
            s =>
                s.ProcessExamNotificationAsync(
                    courseId,
                    examDate,
                    tenantId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithDifferentExamDates_PassesCorrectly()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var dates = new[]
        {
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(30),
            DateTime.UtcNow.AddMonths(3),
        };

        // Act & Assert
        foreach (var examDate in dates)
        {
            var examEvent = new ExamCreatedEvent(courseId, examDate, tenantId);
            var notification = new DomainEventNotification<ExamCreatedEvent>(examEvent);

            await _consumer.Handle(notification, CancellationToken.None);
        }

        _mockService.Verify(
            s =>
                s.ProcessExamNotificationAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Exactly(3)
        );
    }
}

public class RequestApprovalConsumerTests
{
    private readonly Mock<INotificationService> _mockService;
    private readonly RequestApprovalConsumer _consumer;

    public RequestApprovalConsumerTests()
    {
        _mockService = new Mock<INotificationService>();
        _consumer = new RequestApprovalConsumer(_mockService.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequestApprovalEvent_CallsNotificationService()
    {
        // Arrange
        var requestId = 789;
        var requesterId = "user123";
        var requestType = "Transcript";
        var status = "approved";
        var tenantId = Guid.NewGuid();

        var requestEvent = new RequestApprovalEvent(
            requestId,
            requesterId,
            requestType,
            status,
            tenantId
        );
        var notification = new DomainEventNotification<RequestApprovalEvent>(requestEvent);

        // Act
        await _consumer.Handle(notification, CancellationToken.None);

        // Assert
        _mockService.Verify(
            s =>
                s.ProcessRequestApprovalNotificationAsync(
                    requestId,
                    requesterId,
                    requestType,
                    status,
                    tenantId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ExtractsAllEventProperties()
    {
        // Arrange
        var requestId = 456;
        var requesterId = "john.doe";
        var requestType = "Certificate";
        var status = "rejected";
        var tenantId = Guid.NewGuid();

        var requestEvent = new RequestApprovalEvent(
            requestId,
            requesterId,
            requestType,
            status,
            tenantId
        );
        var notification = new DomainEventNotification<RequestApprovalEvent>(requestEvent);

        // Act
        await _consumer.Handle(notification, CancellationToken.None);

        // Assert
        _mockService.Verify(
            s =>
                s.ProcessRequestApprovalNotificationAsync(
                    requestId,
                    requesterId,
                    requestType,
                    status,
                    tenantId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Theory]
    [InlineData("approved")]
    [InlineData("rejected")]
    [InlineData("pending")]
    [InlineData("in_review")]
    public async Task Handle_WithDifferentStatuses_PassesCorrectly(string status)
    {
        // Arrange
        var requestEvent = new RequestApprovalEvent(
            1,
            "user1",
            "Transcript",
            status,
            Guid.NewGuid()
        );
        var notification = new DomainEventNotification<RequestApprovalEvent>(requestEvent);

        // Act
        await _consumer.Handle(notification, CancellationToken.None);

        // Assert
        _mockService.Verify(
            s =>
                s.ProcessRequestApprovalNotificationAsync(
                    1,
                    "user1",
                    "Transcript",
                    status,
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Theory]
    [InlineData("Transcript")]
    [InlineData("Certificate")]
    [InlineData("Enrollment")]
    public async Task Handle_WithDifferentRequestTypes_PassesCorrectly(string requestType)
    {
        // Arrange
        var requestEvent = new RequestApprovalEvent(
            1,
            "user1",
            requestType,
            "approved",
            Guid.NewGuid()
        );
        var notification = new DomainEventNotification<RequestApprovalEvent>(requestEvent);

        // Act
        await _consumer.Handle(notification, CancellationToken.None);

        // Assert
        _mockService.Verify(
            s =>
                s.ProcessRequestApprovalNotificationAsync(
                    1,
                    "user1",
                    requestType,
                    "approved",
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenServiceThrows_PropagatesException()
    {
        // Arrange
        var requestEvent = new RequestApprovalEvent(
            1,
            "user1",
            "Transcript",
            "approved",
            Guid.NewGuid()
        );
        var notification = new DomainEventNotification<RequestApprovalEvent>(requestEvent);
        var exception = new InvalidOperationException("Service error");

        _mockService
            .Setup(s =>
                s.ProcessRequestApprovalNotificationAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _consumer.Handle(notification, CancellationToken.None)
        );
    }
}
