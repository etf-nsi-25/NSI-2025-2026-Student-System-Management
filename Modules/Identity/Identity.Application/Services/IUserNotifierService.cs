using Identity.Core.Entities;

namespace Identity.Application.Services;

public interface IUserNotifierService
{
    public Task SendAccountCreatedNotification(User user, string tempPassword);
}