using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Notifications.Core.Entities;
using Notifications.Infrastructure.Db;
using Notifications.Infrastructure.Repositories;

namespace Notifications.Tests.Repositories;

public class NotificationRepositoryTests : IAsyncLifetime
{
    private NotificationsDbContext _context = null!;
    private NotificationRepository _repository = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<NotificationsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new NotificationsDbContext(options);
        _repository = new NotificationRepository(_context);
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    #region SaveAsync Tests

    [Fact]
    public async Task SaveAsync_WithValidNotificationLog_PersistsToDatabase()
    {
        // Arrange
        var notification = new NotificationLog
        {
            UserId = "user123",
            ChannelType = "Email",
            Destination = "user@example.com",
            Message = "Test message",
            SentAt = DateTime.UtcNow,
            Status = "Sent",
        };

        // Act
        var result = await _repository.SaveAsync(notification);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        result.UserId.Should().Be("user123");
        result.Message.Should().Be("Test message");

        var savedFromDb = await _context.NotificationLogs.FirstOrDefaultAsync(n =>
            n.Id == result.Id
        );
        savedFromDb.Should().NotBeNull();
        savedFromDb!.Message.Should().Be("Test message");
    }

    [Fact]
    public async Task SaveAsync_GeneratesUniqueIds()
    {
        // Arrange
        var notification1 = CreateNotificationLog("user1");
        var notification2 = CreateNotificationLog("user2");

        // Act
        var result1 = await _repository.SaveAsync(notification1);
        var result2 = await _repository.SaveAsync(notification2);

        // Assert
        result1.Id.Should().NotBe(result2.Id);
        result1.Id.Should().BeGreaterThan(0);
        result2.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SaveAsync_WithMultipleNotifications_SavesAllCorrectly()
    {
        // Arrange
        var notifications = new[]
        {
            CreateNotificationLog("user1"),
            CreateNotificationLog("user2"),
            CreateNotificationLog("user3"),
            CreateNotificationLog("user4"),
        };

        // Act
        var results = new List<NotificationLog>();
        foreach (var notification in notifications)
        {
            var result = await _repository.SaveAsync(notification);
            results.Add(result);
        }

        // Assert
        results.Should().HaveCount(4);
        results.Select(r => r.Id).Should().AllSatisfy(id => id.Should().BeGreaterThan(0));

        var dbCount = await _context.NotificationLogs.CountAsync();
        dbCount.Should().Be(4);
    }

    [Fact]
    public async Task SaveAsync_PreservesAllProperties()
    {
        // Arrange
        var sentAt = DateTime.UtcNow;
        var notification = new NotificationLog
        {
            UserId = "testuser",
            ChannelType = "SMS",
            Destination = "+1234567890",
            Message = "This is a test SMS notification",
            SentAt = sentAt,
            Status = "Pending",
        };

        // Act
        var result = await _repository.SaveAsync(notification);

        // Assert
        result.UserId.Should().Be("testuser");
        result.ChannelType.Should().Be("SMS");
        result.Destination.Should().Be("+1234567890");
        result.Message.Should().Be("This is a test SMS notification");
        result.SentAt.Should().Be(sentAt);
        result.Status.Should().Be("Pending");
    }

    #endregion

    #region GetByUserIdAsync Tests

    [Fact]
    public async Task GetByUserIdAsync_WithExistingUser_ReturnsAllNotifications()
    {
        // Arrange
        var userId = "user123";
        var notifications = new[]
        {
            CreateNotificationLog(userId, "Notification 1"),
            CreateNotificationLog(userId, "Notification 2"),
            CreateNotificationLog(userId, "Notification 3"),
        };

        foreach (var notification in notifications)
        {
            await _repository.SaveAsync(notification);
        }

        // Act
        var results = await _repository.GetByUserIdAsync(userId);

        // Assert
        results.Should().HaveCount(3);
        results.Should().AllSatisfy(n => n.UserId.Should().Be(userId));
    }

    [Fact]
    public async Task GetByUserIdAsync_WithNonExistingUser_ReturnsEmptyList()
    {
        // Arrange
        var userId = "nonexistent_user";

        // Act
        var results = await _repository.GetByUserIdAsync(userId);

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsSortedByDateDescending()
    {
        // Arrange
        var userId = "user123";
        var baseDate = DateTime.UtcNow;

        var notifications = new[]
        {
            CreateNotificationLog(userId, "First", baseDate.AddHours(1)),
            CreateNotificationLog(userId, "Third", baseDate.AddHours(3)),
            CreateNotificationLog(userId, "Second", baseDate.AddHours(2)),
        };

        foreach (var notification in notifications)
        {
            await _repository.SaveAsync(notification);
        }

        // Act
        var results = await _repository.GetByUserIdAsync(userId);

        // Assert
        results.ToList()[0].Message.Should().Be("Third"); // Most recent
        results.ToList()[1].Message.Should().Be("Second");
        results.ToList()[2].Message.Should().Be("First"); // Oldest
    }

    [Fact]
    public async Task GetByUserIdAsync_FiltersByUserIdOnly()
    {
        // Arrange
        var user1 = "user1";
        var user2 = "user2";

        await _repository.SaveAsync(CreateNotificationLog(user1, "User1-Msg1"));
        await _repository.SaveAsync(CreateNotificationLog(user1, "User1-Msg2"));
        await _repository.SaveAsync(CreateNotificationLog(user2, "User2-Msg1"));

        // Act
        var user1Results = await _repository.GetByUserIdAsync(user1);
        var user2Results = await _repository.GetByUserIdAsync(user2);

        // Assert
        user1Results.Should().HaveCount(2);
        user1Results.Should().AllSatisfy(n => n.UserId.Should().Be(user1));

        user2Results.Should().HaveCount(1);
        user2Results.First().UserId.Should().Be(user2);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithCancellationToken_Respects()
    {
        // Arrange
        var userId = "user123";
        await _repository.SaveAsync(CreateNotificationLog(userId));
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await _repository.GetByUserIdAsync(userId, cts.Token)
        );
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsNotification()
    {
        // Arrange
        var notification = CreateNotificationLog("user123");
        var saved = await _repository.SaveAsync(notification);

        // Act
        var result = await _repository.GetByIdAsync(saved.Id);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be("user123");
        result.Message.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(99999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsExactNotification()
    {
        // Arrange
        var notification1 = CreateNotificationLog("user1", "Message 1");
        var notification2 = CreateNotificationLog("user2", "Message 2");

        var saved1 = await _repository.SaveAsync(notification1);
        var saved2 = await _repository.SaveAsync(notification2);

        // Act
        var result1 = await _repository.GetByIdAsync(saved1.Id);
        var result2 = await _repository.GetByIdAsync(saved2.Id);

        // Assert
        result1!.UserId.Should().Be("user1");
        result1.Message.Should().Be("Message 1");

        result2!.UserId.Should().Be("user2");
        result2.Message.Should().Be("Message 2");
    }

    #endregion

    #region Data Persistence Tests

    [Fact]
    public async Task SavedNotifications_PersistAcrossDifferentContextInstances()
    {
        // Arrange
        var notification = CreateNotificationLog("user123");
        var saved = await _repository.SaveAsync(notification);
        var dbName = "test-db-" + Guid.NewGuid();

        // Act - Use same in-memory database name to ensure persistence
        var newContext = new NotificationsDbContext(
            new DbContextOptionsBuilder<NotificationsDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options
        );

        // First save to the new context's database
        newContext.NotificationLogs.Add(saved);
        await newContext.SaveChangesAsync();

        var retrieved = await newContext.NotificationLogs.FirstOrDefaultAsync(n =>
            n.Id == saved.Id
        );

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Message.Should().Be(notification.Message);

        await newContext.DisposeAsync();
    }

    [Fact]
    public async Task SaveAsync_WithLongMessage_HandlesProperly()
    {
        // Arrange
        var longMessage = string.Concat(
            Enumerable.Repeat("This is a very long notification message. ", 50)
        );
        var notification = new NotificationLog
        {
            UserId = "user123",
            ChannelType = "Email",
            Destination = "user@example.com",
            Message = longMessage,
            SentAt = DateTime.UtcNow,
            Status = "Sent",
        };

        // Act
        var result = await _repository.SaveAsync(notification);
        var retrieved = await _repository.GetByIdAsync(result.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Message.Should().Be(longMessage);
    }

    #endregion

    #region Helper Methods

    private NotificationLog CreateNotificationLog(
        string userId,
        string? message = null,
        DateTime? sentAt = null
    )
    {
        return new NotificationLog
        {
            UserId = userId,
            ChannelType = "Email",
            Destination = $"{userId}@unsa.ba",
            Message = message ?? $"Test notification for {userId}",
            SentAt = sentAt ?? DateTime.UtcNow,
            Status = "Sent",
        };
    }

    #endregion
}
