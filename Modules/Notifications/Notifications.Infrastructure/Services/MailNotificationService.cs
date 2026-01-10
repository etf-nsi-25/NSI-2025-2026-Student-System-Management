using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Notifications.Application.Services;
using Notifications.Core.Entities;
using Notifications.Infrastructure.Settings;

namespace Notifications.Infrastructure.Services;

public class MailNotificationService(IOptions<NotificationSettings> notificationSettings) : INotificationService
{
    private readonly SmtpSettings _smtpSettings = notificationSettings.Value.Smtp;

    public async Task SendNotification(Notification notification)
    {
        var mailMessage = new MimeMessage();
        mailMessage.Subject = notification.Title;
        mailMessage.From.Add(new MailboxAddress("UNSA SMS",_smtpSettings.From));
        mailMessage.To.AddRange(GetRecipientMailboxAddresses(notification.Recipients));
        mailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = notification.Body };

        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(_smtpSettings.From, _smtpSettings.Password);
        await smtpClient.SendAsync(mailMessage);
        await smtpClient.DisconnectAsync(true);
    }

    private static List<MailboxAddress> GetRecipientMailboxAddresses(ICollection<string> recipients)
    {
        var mailboxAddresses = new List<MailboxAddress>();
        
        var index = 0;
        foreach (var recipient in recipients)
        {
            mailboxAddresses.Add(new MailboxAddress("Recipient " + index, recipient));
            index++;
        }
        
        return mailboxAddresses;
    }
}
