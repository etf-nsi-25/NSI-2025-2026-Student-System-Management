namespace Identity.Core.Services;

public interface IIdentityHasherService
{
    string HashPassword(string password);

    bool VerifyPassword(string password, string hashedPassword);

}