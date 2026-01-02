using Microsoft.EntityFrameworkCore;
using Notifications.Core.Entities;
using Notifications.Core.Interfaces;
using Notifications.Infrastructure.Db;

namespace Notifications.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for NotificationLog persistence.
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly NotificationsDbContext _context;

    public NotificationRepository(NotificationsDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationLog> SaveAsync(
        NotificationLog notificationLog,
        CancellationToken cancellationToken = default
    )
    {
        _context.NotificationLogs.Add(notificationLog);
        await _context.SaveChangesAsync(cancellationToken);
        return notificationLog;
    }

    public async Task<IEnumerable<NotificationLog>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .NotificationLogs.Where(n => n.UserId == userId)
            .OrderByDescending(n => n.SentAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<NotificationLog?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.NotificationLogs.FirstOrDefaultAsync(
            n => n.Id == id,
            cancellationToken
        );
    }
}
