namespace Notifications.Core.Entities;

public record Notification(ICollection<string> Recipients, string Title, string Body)
{
    public Notification(string recipient, string title, string body)
        : this(new List<string> { recipient }, title, body)
    {
    }
}
