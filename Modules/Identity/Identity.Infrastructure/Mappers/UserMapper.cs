using Identity.Core.Entities;

namespace Identity.Infrastructure.Mappers;

public static class UserMapper
{
    public static User ToCore(this ApplicationUser appUser)
    {
        var user = User.Create(
            username: appUser.UserName ?? string.Empty,
            passwordHash: appUser.PasswordHash ?? string.Empty,
            firstName: appUser.FirstName ?? string.Empty,
            lastName: appUser.LastName ?? string.Empty,
            email: appUser.Email ?? string.Empty,
            facultyId: appUser.FacultyId,
            role: appUser.Role,
            indexNumber: appUser.IndexNumber
        );

        // Set the ID from the database entity and update status
        return user.SetId(Guid.Parse(appUser.Id));
    }

    public static ApplicationUser ToInfrastructure(this User user)
    {
        return new ApplicationUser
        {
            Id = user.Id.ToString(),
            UserName = user.Username,
            PasswordHash = user.PasswordHash,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FacultyId = user.FacultyId,
            IndexNumber = user.IndexNumber,
            Role = user.Role,
            Status = user.Status
        };
    }

    public static void UpdateFrom(this ApplicationUser appUser, User user)
    {
        appUser.UserName = user.Username;
        appUser.PasswordHash = user.PasswordHash;
        appUser.FirstName = user.FirstName;
        appUser.LastName = user.LastName;
        appUser.FacultyId = user.FacultyId;
        appUser.IndexNumber = user.IndexNumber;
        appUser.Role = user.Role;
        appUser.Status = user.Status;
    }
}