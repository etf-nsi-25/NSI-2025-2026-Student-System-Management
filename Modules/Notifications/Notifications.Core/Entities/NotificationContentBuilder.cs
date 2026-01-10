using System.Text.RegularExpressions;

namespace Notifications.Core.Entities;

public class NotificationContentBuilder
{
    private string _content;

    private NotificationContentBuilder(string baseContent)
    {
        if (string.IsNullOrWhiteSpace(baseContent))
        {
            throw new ArgumentNullException(nameof(baseContent));
        }

        _content = baseContent;
    }

    public static NotificationContentBuilder FromTemplate(string template)
    {
        return new NotificationContentBuilder(template);
    }

    public NotificationContentBuilder WithPlaceholder(string placeholderName, string placeholderValue)
    {
        var pattern = @"\{\{" + Regex.Escape(placeholderName) + @"\}\}";
        _content = Regex.Replace(_content, pattern, placeholderValue);

        return this;
    }

    public string ToBody()
    {
        return _content;
    }
}
