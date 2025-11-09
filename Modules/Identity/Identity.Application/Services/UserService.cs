using System.Runtime.CompilerServices;
using Identity.Application.DTOs;
using Identity.Core.Entities;
using Identity.Core.Repositories;

// Allow injection.
[assembly: InternalsVisibleTo("Identity.Infrastructure")]
namespace Identity.Application.Services;

internal class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserDTO> CreateUser(string email)
    {
        ApplicationUser result = await userRepository.CreateUser(email);
        await userRepository.Save();
        
        return new UserDTO
        {
            Email = result.UserName,
        };
    }
}
