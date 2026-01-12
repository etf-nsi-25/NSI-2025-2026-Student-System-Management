using Identity.Core.Entities;
using Identity.Core.Services;
using Identity.Infrastructure.Mappers;
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

    public bool VerifyPassword(User user, string password, string hashedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(user.ToInfrastructure(), hashedPassword, password);
        return result == PasswordVerificationResult.Success;
    }
}