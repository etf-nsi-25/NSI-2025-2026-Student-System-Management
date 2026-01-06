using Identity.Core.DTO;
using Identity.Infrastructure.Entities;

namespace Identity.Infrastructure.Mappers;

public static class UserMapper
{
    public static UserResponse MapToResponse(ApplicationUser appUser)
    {
        return new UserResponse
        {
            Id = appUser.Id,
            Username = appUser.UserName ?? string.Empty,
            FirstName = appUser.FirstName ?? string.Empty,
            LastName = appUser.LastName ?? string.Empty,
            IndexNumber = appUser.IndexNumber,
            FacultyId = appUser.FacultyId, 
            Role = appUser.Role,
            Status = appUser.Status,
            Email = appUser.Email ?? string.Empty,
        };
    }

    public static ApplicationUser MapToInfrastructure(CreateUserRequest request)
    {
        return new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            FacultyId = request.FacultyId,
            IndexNumber = request.IndexNumber,
            Role = request.Role,
            Status = Core.Enums.UserStatus.Active
        };
    }
}