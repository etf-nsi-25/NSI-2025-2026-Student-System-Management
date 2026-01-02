using Notifications.Core.Entities;

namespace Notifications.Core.Interfaces;

/// <summary>
/// Repository interface for NotificationLog persistence operations.
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// Saves a notification log entry to the database.
    /// </summary>
    /// <param name="notificationLog">The notification log to save.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The saved notification log with its generated ID.</returns>
    Task<NotificationLog> SaveAsync(
        NotificationLog notificationLog,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves all notification logs for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of notification logs.</returns>
    Task<IEnumerable<NotificationLog>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a notification log by its ID.
    /// </summary>
    /// <param name="id">The notification log ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The notification log or null if not found.</returns>
    Task<NotificationLog?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
