using Identity.Core.DTO;
using Identity.Core.Enums;

namespace Identity.Application.Interfaces;

public interface IUserService
{
    Task<string> CreateUserAsync(
           string username,
           string password,
           string firstName,
           string lastName,
           string email,
           Guid facultyId,
           string? indexNumber,
           UserRole role
       );

    Task<UserListResponse> GetAllUsersAsync(UserFilterRequest filter);



    Task<bool> DeleteUserAsync(string userId);

    Task<UserResponse?> GetUserByIdAsync(string userId);

    Task<bool> UpdateUserAsync(string userId, UpdateUserRequest request);

    Task<bool> DeactivateUserAsync(string userId);

    Task<bool> ChangePasswordAsync(string userId, string newPassword);

    Task<int> CountUsers(UserFilterRequest filter);
}