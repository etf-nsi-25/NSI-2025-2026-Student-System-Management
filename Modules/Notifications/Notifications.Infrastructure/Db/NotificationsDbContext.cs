using Microsoft.EntityFrameworkCore;
using Notifications.Core.Entities;

namespace Notifications.Infrastructure.Db;

/// <summary>
/// Database context for the Notifications module.
/// This module is autonomous and maintains its own database context.
/// </summary>
public class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
        : base(options)
    {
    }

    public DbSet<NotificationLog> NotificationLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureNotificationLog(modelBuilder);
    }

    private void ConfigureNotificationLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationLog>(entity =>
        {
            entity.ToTable("NotificationLog");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.ChannelType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Destination).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.SentAt).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);

            // Indexes for common queries
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.SentAt);
        });
    }
}
