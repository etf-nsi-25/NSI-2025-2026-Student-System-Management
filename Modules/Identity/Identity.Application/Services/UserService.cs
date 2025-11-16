using System.Runtime.CompilerServices;
using Identity.Core.Entities;
using Identity.Core.Interfaces.Repositories;
using Identity.Core.Interfaces.Services;


// Allow injection.
[assembly: InternalsVisibleTo("Identity.Infrastructure")]
namespace Identity.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<User> CreateUser(string email)
    {
        var user = await userRepository.CreateUser(email);
        await userRepository.Save();
        
        return user;
    }
}
