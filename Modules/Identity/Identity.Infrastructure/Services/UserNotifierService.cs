using Identity.Application.Services;
using Identity.Core.Entities;
using Notifications.Application.Services;
using Notifications.Core.Entities;

namespace Identity.Infrastructure.Services;

public class UserNotifierService(INotificationService notificationService) : IUserNotifierService
{
    private const string UserName = "UserName";
    private const string TemporaryPassword = "TemporaryPassword";

    public async Task SendAccountCreatedNotification(string email, string tempPassword)
    {
        var bodyTemplate = await File.ReadAllTextAsync(Path.Combine(
            AppContext.BaseDirectory,
            "Templates",
            "new-user-template.html"
        ));

        var body = NotificationContentBuilder.FromTemplate(bodyTemplate)
            .WithPlaceholder(UserName, ExtractUsernameFromEmail(email))
            .WithPlaceholder(TemporaryPassword, tempPassword)
            .ToBody();

        await notificationService.SendNotification(new Notification(
            email,
            "UNSA | Account Created",
            body
        ));
    }

    private static string ExtractUsernameFromEmail(string email) => email[..email.IndexOf('@')];
}
