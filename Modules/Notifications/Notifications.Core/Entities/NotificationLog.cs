using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notifications.Core.Entities;

/// <summary>
/// Represents a notification log entry.
/// Stores all notification attempts for audit and tracking purposes.
/// </summary>
public class NotificationLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// The Identity ID of the recipient user.
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The channel type (e.g., "Email", "SMS").
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ChannelType { get; set; } = string.Empty;

    /// <summary>
    /// The destination address (e.g., user@unsa.ba for email).
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Destination { get; set; } = string.Empty;

    /// <summary>
    /// The notification message content.
    /// </summary>
    [Required]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the notification was sent or attempted.
    /// </summary>
    [Required]
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Status of the notification (e.g., "Sent", "Pending", "Failed").
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
}
