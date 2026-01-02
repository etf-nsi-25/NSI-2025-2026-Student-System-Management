using Common.Core.Events;
using Common.Core.Tenant;
using Common.Infrastructure.DependencyInjection;
using EventBus.Core;
using EventBus.Infrastructure;
using EventBus.MediatR;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Notifications.Application.Services;
using Notifications.Core.Entities;
using Notifications.Core.Interfaces;
using Notifications.Infrastructure.Db;
using Notifications.Infrastructure.EventHandlers;
using Notifications.Infrastructure.Repositories;

namespace Notifications.Tests.Integration;

public class NotificationEventFlowIntegrationTests : IAsyncLifetime
{
    private ServiceProvider _serviceProvider = null!;
    private IServiceScope _scope = null!;
    private NotificationsDbContext _dbContext = null!;
    private IEventBus _eventBus = null!;
    private INotificationRepository _repository = null!;

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();

        // Add DbContext with In-Memory database
        var dbName = Guid.NewGuid().ToString();
        services.AddDbContext<NotificationsDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        // Add repositories
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // Add services
        services.AddScoped<INotificationService, NotificationService>();

        // Add logging
        services.AddLogging();

        // Add common module (includes IScopedTenantContext)
        services.AddCommonModule();

        // Add event bus
        services.AddEventBus();

        // Add event handlers/consumers
        services.AddScoped<GradePostedConsumer>();
        services.AddScoped<ExamCreatedConsumer>();
        services.AddScoped<RequestApprovalConsumer>();

        _serviceProvider = services.BuildServiceProvider();
        _scope = _serviceProvider.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        _eventBus = _scope.ServiceProvider.GetRequiredService<IEventBus>();
        _repository = _scope.ServiceProvider.GetRequiredService<INotificationRepository>();

        // Initialize database
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        _scope?.Dispose();
        _serviceProvider?.Dispose();
    }

    #region Grade Notification Integration Tests

    [Fact]
    public async Task GradePostedEvent_IsPublishedAndProcessed()
    {
        // Arrange
        var studentId = 123;
        var courseName = "Mathematics";
        var grade = 8.5;
        var tenantId = Guid.NewGuid();

        var gradeEvent = new GradePostedEvent(studentId, courseName, grade, tenantId);

        // Act
        await _eventBus.Dispatch(gradeEvent, tenantId);

        // Assert
        var notifications = await _repository.GetByUserIdAsync(studentId.ToString());
        notifications.Should().NotBeEmpty();

        var notification = notifications.First();
        notification.Message.Should().Contain(courseName);
        notification.Message.Should().Contain("8.5");
        notification.Status.Should().Be("Sent");
    }

    [Fact]
    public async Task MultipleGradeEvents_CreateMultipleNotifications()
    {
        // Arrange
        var studentId = 456;
        var tenantId = Guid.NewGuid();
        var events = new[]
        {
            new GradePostedEvent(studentId, "Physics", 7.5, tenantId),
            new GradePostedEvent(studentId, "Chemistry", 8.0, tenantId),
            new GradePostedEvent(studentId, "Biology", 9.0, tenantId)
        };

        // Act
        foreach (var gradeEvent in events)
        {
            await _eventBus.Dispatch(gradeEvent, tenantId);
        }

        // Assert
        var notifications = await _repository.GetByUserIdAsync(studentId.ToString());
        notifications.Should().HaveCount(3);
        notifications.Should().AllSatisfy(n => 
            n.UserId.Should().Be(studentId.ToString()));
    }

    [Fact]
    public async Task GradeEvent_CreatesCorrectDatabaseEntry()
    {
        // Arrange
        var gradeEvent = new GradePostedEvent(789, "English", 6.5, Guid.NewGuid());

        // Act
        await _eventBus.Dispatch(gradeEvent);

        // Assert
        var dbNotifications = await _dbContext.NotificationLogs
            .Where(n => n.UserId == "789")
            .ToListAsync();

        dbNotifications.Should().HaveCount(1);
        var notification = dbNotifications.First();
        notification.ChannelType.Should().Be("Email");
        notification.Destination.Should().Contain("student789");
        notification.Message.Should().Contain("English");
        notification.Message.Should().Contain("6.5");
    }

    #endregion

    #region Exam Notification Integration Tests

    [Fact]
    public async Task ExamCreatedEvent_IsPublishedAndProcessed()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var examDate = DateTime.UtcNow.AddDays(10);
        var tenantId = Guid.NewGuid();

        var examEvent = new ExamCreatedEvent(courseId, examDate, tenantId);

        // Act
        await _eventBus.Dispatch(examEvent, tenantId);

        // Assert
        var notifications = await _dbContext.NotificationLogs
            .Where(n => n.Message.Contains(courseId.ToString()))
            .ToListAsync();

        notifications.Should().NotBeEmpty();
        var notification = notifications.First();
        notification.Message.Should().Contain(examDate.ToString("yyyy-MM-dd HH:mm"));
    }

    [Fact]
    public async Task ExamEvent_ContainsAllCriticalInformation()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var examDate = new DateTime(2026, 4, 20, 14, 00, 0, DateTimeKind.Utc);
        var tenantId = Guid.NewGuid();

        var examEvent = new ExamCreatedEvent(courseId, examDate, tenantId);

        // Act
        await _eventBus.Dispatch(examEvent, tenantId);

        // Assert
        var notification = await _dbContext.NotificationLogs
            .FirstAsync(n => n.Message.Contains(courseId.ToString()));

        notification.Message.Should().Contain("exam");
        notification.Message.Should().Contain("2026-04-20 14:00");
        notification.Message.Should().Contain("register");
        notification.Message.Should().Contain("deadline");
    }

    #endregion

    #region Request Approval Notification Integration Tests

    [Fact]
    public async Task RequestApprovalEvent_WithApprovedStatus_IsProcessed()
    {
        // Arrange
        var requestId = 999;
        var requesterId = "student456";
        var requestType = "Transcript";
        var tenantId = Guid.NewGuid();

        var requestEvent = new RequestApprovalEvent(requestId, requesterId, requestType, "approved", tenantId);

        // Act
        await _eventBus.Dispatch(requestEvent, tenantId);

        // Assert
        var notifications = await _repository.GetByUserIdAsync(requesterId);
        notifications.Should().NotBeEmpty();

        var notification = notifications.First();
        notification.Message.Should().Contain("approved");
        notification.Message.Should().Contain(requestType);
    }

    [Fact]
    public async Task MultipleRequestEvents_WithDifferentStatuses_AreProcessed()
    {
        // Arrange
        var requesterId = "user789";
        var tenantId = Guid.NewGuid();
        var statuses = new[] { "pending", "approved", "rejected" };

        // Act
        for (int i = 0; i < statuses.Length; i++)
        {
            var requestEvent = new RequestApprovalEvent(100 + i, requesterId, "Certificate", statuses[i], tenantId);
            await _eventBus.Dispatch(requestEvent, tenantId);
        }

        // Assert
        var notifications = await _repository.GetByUserIdAsync(requesterId);
        notifications.Should().HaveCount(3);

        var pendingNotifs = notifications.Where(n => n.Message.Contains("pending")).ToList();
        var approvedNotifs = notifications.Where(n => n.Message.Contains("approved")).ToList();
        var rejectedNotifs = notifications.Where(n => n.Message.Contains("rejected")).ToList();

        pendingNotifs.Should().HaveCount(1);
        approvedNotifs.Should().HaveCount(1);
        rejectedNotifs.Should().HaveCount(1);
    }

    #endregion

    #region Multi-Event Integration Tests

    [Fact]
    public async Task MixedEvents_AreProcessedCorrectly()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        var gradeEvent = new GradePostedEvent(111, "Math", 8.0, tenantId);
        var examEvent = new ExamCreatedEvent(Guid.NewGuid(), DateTime.UtcNow.AddDays(5), tenantId);
        var requestEvent = new RequestApprovalEvent(222, "user222", "Transcript", "approved", tenantId);

        // Act
        await _eventBus.Dispatch(gradeEvent, tenantId);
        await _eventBus.Dispatch(examEvent, tenantId);
        await _eventBus.Dispatch(requestEvent, tenantId);

        // Assert
        var totalNotifications = await _dbContext.NotificationLogs.CountAsync();
        totalNotifications.Should().Be(3);

        var gradeNotifs = await _dbContext.NotificationLogs
            .Where(n => n.Message.Contains("Math"))
            .ToListAsync();
        gradeNotifs.Should().HaveCount(1);

        var examNotifs = await _dbContext.NotificationLogs
            .Where(n => n.Message.Contains("exam"))
            .ToListAsync();
        examNotifs.Should().HaveCount(1);

        var requestNotifs = await _dbContext.NotificationLogs
            .Where(n => n.Message.Contains("Transcript"))
            .ToListAsync();
        requestNotifs.Should().HaveCount(1);
    }

    [Fact]
    public async Task ConcurrentEvents_AreProcessedCorrectly()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tasks = new List<Task>();

        // Create multiple events
        for (int i = 0; i < 10; i++)
        {
            var gradeEvent = new GradePostedEvent(i, $"Course{i}", 5.0 + i * 0.5, tenantId);
            tasks.Add(_eventBus.Dispatch(gradeEvent, tenantId));
        }

        // Act
        await Task.WhenAll(tasks);

        // Assert
        var notificationCount = await _dbContext.NotificationLogs.CountAsync();
        notificationCount.Should().Be(10);

        var allNotifications = await _dbContext.NotificationLogs.ToListAsync();
        allNotifications.Should().AllSatisfy(n => n.Status.Should().Be("Sent"));
    }

    #endregion

    #region Database Persistence Tests

    [Fact]
    public async Task NotificationLog_IsPersistsCorrectly()
    {
        // Arrange
        var gradeEvent = new GradePostedEvent(333, "History", 7.5, Guid.NewGuid());

        // Act
        await _eventBus.Dispatch(gradeEvent);

        // Assert - Query database directly
        var dbEntry = await _dbContext.NotificationLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.UserId == "333");

        dbEntry.Should().NotBeNull();
        dbEntry!.Id.Should().BeGreaterThan(0);
        dbEntry.ChannelType.Should().Be("Email");
        dbEntry.Status.Should().Be("Sent");
    }

    [Fact]
    public async Task NotificationLog_ContainsAllRequiredFields()
    {
        // Arrange
        var gradeEvent = new GradePostedEvent(444, "Biology", 8.5, Guid.NewGuid());

        // Act
        await _eventBus.Dispatch(gradeEvent);

        // Assert
        var notification = await _dbContext.NotificationLogs
            .FirstAsync(n => n.UserId == "444");

        notification.Id.Should().BeGreaterThan(0);
        notification.UserId.Should().NotBeNullOrEmpty();
        notification.ChannelType.Should().NotBeNullOrEmpty();
        notification.Destination.Should().NotBeNullOrEmpty();
        notification.Message.Should().NotBeNullOrEmpty();
        notification.SentAt.Should().NotBe(default(DateTime));
        notification.Status.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region Event Bus Tenant Awareness Tests

    [Fact]
    public async Task Event_WithDifferentTenants_IsProcessedWithTenantId()
    {
        // Arrange
        var tenant1 = Guid.NewGuid();
        var tenant2 = Guid.NewGuid();

        var event1 = new GradePostedEvent(555, "Math", 8.0, tenant1);
        var event2 = new GradePostedEvent(555, "English", 7.0, tenant2);

        // Act
        await _eventBus.Dispatch(event1, tenant1);
        await _eventBus.Dispatch(event2, tenant2);

        // Assert
        var notifications = await _dbContext.NotificationLogs
            .Where(n => n.UserId == "555")
            .ToListAsync();

        notifications.Should().HaveCount(2);
        notifications.Should().AllSatisfy(n => n.Message.Should().NotBeNullOrEmpty());
    }

    #endregion
}

/// <summary>
/// Simple disposable stub for mocking tenant scope context
/// </summary>
internal sealed class DisposableStub : IDisposable
{
    public void Dispose()
    {
        // Stub implementation
    }
}
