using Identity.Core.DTO;
using Identity.Core.Enums;

namespace Identity.Application.Interfaces;

public interface IUserService
{
    Task<string> CreateUserAsync(
           string username,
           string firstName,
           string lastName,
           string email,
           Guid facultyId,
           string? indexNumber,
           UserRole role,
           UserRole? requesterRole = null
       );

    Task<UserListResponse> GetAllUsersAsync(UserFilterRequest filter);



    Task<bool> DeleteUserAsync(string userId, UserRole? requesterRole = null);
    Task<UserResponse?> GetUserByIdAsync(string userId);
    Task<bool> UpdateUserAsync(string userId, UpdateUserRequest request, UserRole? requesterRole = null);
    Task<bool> DeactivateUserAsync(string userId);

    Task<bool> ChangePasswordAsync(string userId, string newPassword);

    Task<int> CountUsers(UserFilterRequest filter);
}