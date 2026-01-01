using Identity.Core.DTO;
using Identity.Core.Enums;

namespace Identity.Application.Interfaces;

public interface IUserService
{
    Task<Guid> CreateUserAsync(
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



    Task<bool> DeleteUserAsync(Guid userId);

    Task<UserResponse?> GetUserByIdAsync(Guid userId);

    Task<bool> UpdateUserAsync(Guid userId, UpdateUserRequest request);

    Task<bool> DeactivateUserAsync(Guid userId);

}