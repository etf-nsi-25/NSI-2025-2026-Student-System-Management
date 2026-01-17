namespace Identity.Application.Services;

public interface IUserNotifierService
{
    public Task SendAccountCreatedNotification(string email, string tempPassword);
}
