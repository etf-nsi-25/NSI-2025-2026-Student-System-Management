using Identity.Core.Entities;
using Identity.Infrastructure.Entities;

namespace Identity.Infrastructure.Mappers;

public static class UserMapper
{
    public static User ToCore(this ApplicationUser appUser)
    {
        return new User(Guid.Parse(appUser.Id),appUser.UserName,Email: appUser.Email,PasswordHash: appUser.PasswordHash,"Role", Guid.NewGuid());
    }

    public static ApplicationUser ToInfrastructure(this User user)
    {
        return new ApplicationUser
        {
            Id = user.Id.ToString(),
            UserName = user.FullName,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
        };
    }

    public static void UpdateFrom(this ApplicationUser appUser, User user)
    {
        appUser.Email = user.Email;
        appUser.UserName = user.Email;

    }
}