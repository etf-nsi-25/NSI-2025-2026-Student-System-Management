using System.Runtime.CompilerServices;
using Identity.Core.Entities;
using Identity.Core.Repositories;

// Allow injection.
[assembly: InternalsVisibleTo("Identity.Infrastructure")]
namespace Identity.Application.Services;

internal class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<User> CreateUser(string email)
    {
        var user = await userRepository.CreateUser(email);
        await userRepository.Save();
        
        return user;
    }
}
