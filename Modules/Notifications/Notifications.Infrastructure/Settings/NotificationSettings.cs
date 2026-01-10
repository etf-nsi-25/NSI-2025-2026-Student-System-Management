namespace Notifications.Infrastructure.Settings;

public class NotificationSettings
{
    public NotificationEngine NotificationEngineType { get; set; }
    public SmtpSettings Smtp { get; set; } = new();
    
    public enum NotificationEngine
    {
        Mock, 
        Mail
    }
}
