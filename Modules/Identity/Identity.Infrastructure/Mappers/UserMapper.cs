using Identity.Core.Entities;
using Identity.Infrastructure.Entities;

namespace Identity.Infrastructure.Mappers;

public static class UserMapper
{
    public static User ToCore(this ApplicationUser appUser)
    {
        return new User(Email: appUser.Email, Id: appUser.Id);
    }

    public static ApplicationUser ToInfrastructure(this User user)
    {
        return new ApplicationUser
        {
            Id = user.Id,
            UserName = user.Email,
            Email = user.Email
        };
    }

    public static void UpdateFrom(this ApplicationUser appUser, User user)
    {
        appUser.Email = user.Email;
        appUser.UserName = user.Email;

    }
}