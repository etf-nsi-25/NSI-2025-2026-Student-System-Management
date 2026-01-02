using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Notifications.Core.Entities;
using Notifications.Infrastructure.Db;

namespace Notifications.Tests.Database;

public class NotificationsDbContextTests : IAsyncLifetime
{
    private NotificationsDbContext _context = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<NotificationsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new NotificationsDbContext(options);
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    #region DbSet Configuration Tests

    [Fact]
    public void NotificationLogs_DbSet_IsConfigured()
    {
        // Assert
        _context.NotificationLogs.Should().NotBeNull();
        _context.NotificationLogs.Should().BeAssignableTo<DbSet<NotificationLog>>();
    }

    [Fact]
    public async Task AddNotificationLog_InsertsSuccessfully()
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
        _context.NotificationLogs.Add(notification);
        await _context.SaveChangesAsync();

        // Assert
        var savedNotification = await _context.NotificationLogs.FirstOrDefaultAsync(n =>
            n.UserId == "user123"
        );

        savedNotification.Should().NotBeNull();
        savedNotification!.Message.Should().Be("Test message");
    }

    #endregion

    #region Entity Configuration Tests

    [Fact]
    public async Task NotificationLog_TableNameIsCorrect()
    {
        // Arrange
        var notification = new NotificationLog
        {
            UserId = "test_user",
            ChannelType = "Email",
            Destination = "test@test.com",
            Message = "Test",
            SentAt = DateTime.UtcNow,
            Status = "Sent",
        };

        _context.NotificationLogs.Add(notification);
        await _context.SaveChangesAsync();

        // Act
        var tableName = _context.Model.FindEntityType(typeof(NotificationLog))?.GetTableName();

        // Assert
        tableName.Should().Be("NotificationLog");
    }

    [Fact]
    public async Task NotificationLog_IdProperty_IsKeyAndAutoIncrement()
    {
        // Arrange
        var notification1 = new NotificationLog
        {
            UserId = "user1",
            ChannelType = "Email",
            Destination = "user1@test.com",
            Message = "Message1",
            SentAt = DateTime.UtcNow,
            Status = "Sent",
        };

        var notification2 = new NotificationLog
        {
            UserId = "user2",
            ChannelType = "Email",
            Destination = "user2@test.com",
            Message = "Message2",
            SentAt = DateTime.UtcNow,
            Status = "Sent",
        };

        // Act
        _context.NotificationLogs.Add(notification1);
        _context.NotificationLogs.Add(notification2);
        await _context.SaveChangesAsync();

        // Assert
        notification1.Id.Should().BeGreaterThan(0);
        notification2.Id.Should().BeGreaterThan(notification1.Id);
        notification1.Id.Should().NotBe(notification2.Id);
    }

    [Fact]
    public async Task NotificationLog_AllPropertiesArePersisted()
    {
        // Arrange
        var sentAt = DateTime.UtcNow;
        var notification = new NotificationLog
        {
            UserId = "user456",
            ChannelType = "SMS",
            Destination = "+1234567890",
            Message = "SMS notification content",
            SentAt = sentAt,
            Status = "Pending",
        };

        // Act
        _context.NotificationLogs.Add(notification);
        await _context.SaveChangesAsync();

        var retrieved = await _context.NotificationLogs.FirstOrDefaultAsync(n =>
            n.UserId == "user456"
        );

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.UserId.Should().Be("user456");
        retrieved.ChannelType.Should().Be("SMS");
        retrieved.Destination.Should().Be("+1234567890");
        retrieved.Message.Should().Be("SMS notification content");
        retrieved.SentAt.Should().Be(sentAt);
        retrieved.Status.Should().Be("Pending");
    }

    #endregion

    #region Index Tests

    [Fact]
    public void NotificationLog_HasIndexOnUserId()
    {
        // Arrange & Act
        var entityType = _context.Model.FindEntityType(typeof(NotificationLog));
        var indexes = entityType?.GetIndexes();

        // Assert
        indexes.Should().NotBeNull();
        var userIdIndex = indexes?.FirstOrDefault(i => i.Properties.Any(p => p.Name == "UserId"));
        userIdIndex.Should().NotBeNull();
    }

    [Fact]
    public void NotificationLog_HasIndexOnStatus()
    {
        // Arrange & Act
        var entityType = _context.Model.FindEntityType(typeof(NotificationLog));
        var indexes = entityType?.GetIndexes();

        // Assert
        var statusIndex = indexes?.FirstOrDefault(i => i.Properties.Any(p => p.Name == "Status"));
        statusIndex.Should().NotBeNull();
    }

    [Fact]
    public void NotificationLog_HasIndexOnSentAt()
    {
        // Arrange & Act
        var entityType = _context.Model.FindEntityType(typeof(NotificationLog));
        var indexes = entityType?.GetIndexes();

        // Assert
        var sentAtIndex = indexes?.FirstOrDefault(i => i.Properties.Any(p => p.Name == "SentAt"));
        sentAtIndex.Should().NotBeNull();
    }

    #endregion

    #region Data Integrity Tests

    [Fact]
    public async Task NotificationLog_RequiredFields_CannotBeNull()
    {
        // Arrange
        var notification = new NotificationLog
        {
            UserId = null!, // This should fail
            ChannelType = "Email",
            Destination = "test@test.com",
            Message = "Test",
            SentAt = DateTime.UtcNow,
            Status = "Sent",
        };

        _context.NotificationLogs.Add(notification);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => await _context.SaveChangesAsync());
    }

    [Fact]
    public async Task NotificationLog_SupportsLongMessages()
    {
        // Arrange
        var longMessage = string.Concat(Enumerable.Repeat("This is a long message. ", 100));
        var notification = new NotificationLog
        {
            UserId = "user789",
            ChannelType = "Email",
            Destination = "user@test.com",
            Message = longMessage,
            SentAt = DateTime.UtcNow,
            Status = "Sent",
        };

        // Act
        _context.NotificationLogs.Add(notification);
        await _context.SaveChangesAsync();

        var retrieved = await _context.NotificationLogs.FirstAsync(n => n.UserId == "user789");

        // Assert
        retrieved.Message.Should().Be(longMessage);
    }

    [Fact]
    public async Task NotificationLog_CanStoreMaxLengthStrings()
    {
        // Arrange
        var notification = new NotificationLog
        {
            UserId = new string('a', 450), // Max length
            ChannelType = new string('b', 50), // Max length
            Destination = new string('c', 500), // Max length
            Message = "Test message",
            SentAt = DateTime.UtcNow,
            Status = new string('d', 50), // Max length
        };

        // Act
        _context.NotificationLogs.Add(notification);
        await _context.SaveChangesAsync();

        var retrieved = await _context.NotificationLogs.FirstAsync(n =>
            n.Message == "Test message"
        );

        // Assert
        retrieved.UserId.Length.Should().Be(450);
        retrieved.ChannelType.Length.Should().Be(50);
        retrieved.Destination.Length.Should().Be(500);
        retrieved.Status.Length.Should().Be(50);
    }

    #endregion

    #region Query Performance Tests

    [Fact]
    public async Task QueryByUserId_WithIndex_PerformsEfficiently()
    {
        // Arrange
        var userId = "perf_test_user";
        for (int i = 0; i < 100; i++)
        {
            _context.NotificationLogs.Add(
                new NotificationLog
                {
                    UserId = i == 50 ? userId : $"user_{i}",
                    ChannelType = "Email",
                    Destination = $"user{i}@test.com",
                    Message = $"Message {i}",
                    SentAt = DateTime.UtcNow,
                    Status = "Sent",
                }
            );
        }
        await _context.SaveChangesAsync();

        // Act - Query should use index
        var results = await _context.NotificationLogs.Where(n => n.UserId == userId).ToListAsync();

        // Assert
        results.Should().HaveCount(1);
        results.First().Message.Should().Contain("Message 50");
    }

    [Fact]
    public async Task QueryByStatus_WithIndex_PerformsEfficiently()
    {
        // Arrange
        for (int i = 0; i < 50; i++)
        {
            _context.NotificationLogs.Add(
                new NotificationLog
                {
                    UserId = $"user_{i}",
                    ChannelType = "Email",
                    Destination = $"user{i}@test.com",
                    Message = $"Message {i}",
                    SentAt = DateTime.UtcNow,
                    Status = i % 2 == 0 ? "Sent" : "Pending",
                }
            );
        }
        await _context.SaveChangesAsync();

        // Act
        var sentNotifications = await _context
            .NotificationLogs.Where(n => n.Status == "Sent")
            .ToListAsync();

        var pendingNotifications = await _context
            .NotificationLogs.Where(n => n.Status == "Pending")
            .ToListAsync();

        // Assert
        sentNotifications.Should().HaveCount(25);
        pendingNotifications.Should().HaveCount(25);
    }

    #endregion
}
