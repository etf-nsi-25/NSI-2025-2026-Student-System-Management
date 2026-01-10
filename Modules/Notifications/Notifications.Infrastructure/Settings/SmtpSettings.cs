namespace Notifications.Infrastructure.Settings;

public class SmtpSettings
{
    public string From { get; set; }
    public string Password { get; set; }
    public string Server {  get; set; }
    public int Port { get; set; }
}
