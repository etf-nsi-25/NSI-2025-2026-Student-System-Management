using System.Runtime.CompilerServices;
using EventBus.Core;
using Identity.Application.Interfaces;
using Identity.Core.Events;
using Identity.Core.Enums;
using Identity.Core.DTO;
using Identity.Core.Interfaces.Services;

[assembly: InternalsVisibleTo("Identity.Infrastructure"), InternalsVisibleTo("Identity.API")]
namespace Identity.Application.Services;

internal class UserService(
    IIdentityService identityService,
    IEventBus eventBus) : IUserService
{
    public async Task<string> CreateUserAsync(
       string username,
       string password,
       string firstName,
       string lastName,
       string email,
       Guid facultyId,
       string? indexNumber,
       UserRole role)
    {
        if (role == UserRole.Superadmin || role == UserRole.Admin)
        {
            throw new InvalidOperationException("Admin users are restricted from assigning Superadmin or Admin roles.");
        }

        var existingUser = await identityService.FindByEmailAsync(email);
        if (existingUser != null)
        {
            throw new ArgumentException($"User with email '{email}' is already taken.", nameof(email));
        }

        var createRequest = new CreateUserRequest
        {
            Username = username,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            FacultyId = facultyId,
            IndexNumber = indexNumber,
            Role = role,
            Password = password
        };

        var (success, errors) = await identityService.CreateUserAsync(createRequest, password);

        if (!success)
        {
            throw new Exception($"User creation failed: {string.Join(", ", errors)}");
        }

        var createdUser = await identityService.FindByEmailAsync(email);
        if (createdUser == null) throw new Exception("User retrieval failed.");

        await eventBus.Dispatch(new UserCreatedEvent(
            createdUser.Id,
            createdUser.Username,
            createdUser.FirstName,
            createdUser.LastName,
            createdUser.FacultyId,
            createdUser.Role,
            createdUser.IndexNumber
        ), facultyId);

        return createdUser.Id;
    }

    public async Task<UserListResponse> GetAllUsersAsync(UserFilterRequest filter)
    {
        var userResponses = await identityService.GetAllFilteredAsync(filter);
        var totalCount = await identityService.CountAsync(filter);

        return new UserListResponse
        {
            Items = userResponses.ToList(),
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<UserResponse?> GetUserByIdAsync(string userId)
    {
        return await identityService.FindByIdAsync(userId);
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await identityService.FindByIdAsync(userId);
        if (user == null) return false;

        if (user.Role == UserRole.Superadmin)
        {
            throw new InvalidOperationException("Cannot delete Superadmin user.");
        }

        var result = await identityService.DeleteUserAsync(userId);
        if (result)
        {
            await eventBus.Dispatch(new UserDeletedEvent(userId));
        }
        return result;
    }

    public async Task<bool> UpdateUserAsync(string userId, UpdateUserRequest request)
    {
        var user = await identityService.FindByIdAsync(userId);
        if (user == null) return false;

        var previousRole = user.Role;

        if (request.Role == UserRole.Superadmin || request.Role == UserRole.Admin)
        {
            throw new InvalidOperationException("Admin users are restricted from assigning Superadmin or Admin roles.");
        }

        request.Id = userId;
        var result = await identityService.UpdateUserAsync(request);

        if (result && previousRole != request.Role)
        {
            await eventBus.Dispatch(new UserRoleAssignedEvent(userId, previousRole, request.Role));
        }

        return result;
    }

    public async Task<bool> DeactivateUserAsync(string userId)
    {
        var user = await identityService.FindByIdAsync(userId);
        if (user == null) return false;

        if (user.Role == UserRole.Superadmin)
        {
            throw new InvalidOperationException("Cannot deactivate Superadmin user.");
        }

        var updateRequest = new UpdateUserRequest
        {
            Id = userId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Email = user.Email,
            FacultyId = user.FacultyId,
            Role = user.Role,
            IndexNumber = user.IndexNumber,
            Status = UserStatus.Inactive
        };

        return await identityService.UpdateUserAsync(updateRequest);
    }

    public async Task<bool> ChangePasswordAsync(string userId, string newPassword)
    {
        var user = await identityService.FindByIdAsync(userId);
        if (user == null) return false;

        var updateRequest = new UpdateUserRequest
        {
            Id = userId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Email = user.Email,
            FacultyId = user.FacultyId,
            Role = user.Role,
            Status = user.Status,
            IndexNumber = user.IndexNumber,
            Password = newPassword
        };

        return await identityService.UpdateUserAsync(updateRequest);
    }

    public async Task<int> CountUsers(UserFilterRequest filter)
    {
        return await identityService.CountAsync(filter);
    }
}