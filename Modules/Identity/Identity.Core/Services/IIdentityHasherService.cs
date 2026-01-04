using Identity.Core.Entities;

namespace Identity.Core.Services;

public interface IIdentityHasherService
{
    string HashPassword(string password);

    bool VerifyPassword(User user, string password, string hashedPassword);

}