using Identity.Core.Services;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services;


public class IdentityHasherService : IIdentityHasherService
{
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

    public IdentityHasherService(IPasswordHasher<ApplicationUser> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(new ApplicationUser(), password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(new ApplicationUser(), hashedPassword, password);
        return result == PasswordVerificationResult.Success;
    }
}