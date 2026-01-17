using Identity.Core.DTO;
using Identity.Core.Enums;

namespace Identity.Core.Interfaces.Services;

public interface IIdentityService
{
    Task<UserResponse?> FindByEmailAsync(string email);
    
    Task<UserResponse?> FindByIdAsync(string userId);

    Task<bool> CheckPasswordAsync(string userId, string password);

    Task<(bool Success, string[] Errors)> CreateUserAsync(CreateUserRequest request, string password);

    Task<bool> UpdateUserAsync(UpdateUserRequest request);

    Task<bool> DeleteUserAsync(string userId);

    Task<IEnumerable<UserResponse>> GetAllFilteredAsync(UserFilterRequest filter);

    Task<int> CountAsync(UserFilterRequest filter);
}