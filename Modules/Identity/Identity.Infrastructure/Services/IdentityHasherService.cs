using Identity.Core.Services;
using Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services;


internal class IdentityHasherService : IIdentityHasherService
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
}