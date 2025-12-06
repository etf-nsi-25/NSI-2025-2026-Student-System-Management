using Identity.Core.Enums;

namespace Identity.Core.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Guid FacultyId { get; private set; }
    public string? IndexNumber { get; private set; }
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.Active;

    internal User SetId(Guid id)
    {
        this.Id = id;
        return this;
    }

    public void FullUpdate(
        string firstName,
        string lastName,
        Guid facultyId,
        UserRole role,
        UserStatus status,
        string? indexNumber = null
    )
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.FacultyId = facultyId;

        this.Role = role;
        this.Status = status;

        if (role == UserRole.Student)
        {
            this.IndexNumber = indexNumber;
        }
        else
        {
            this.IndexNumber = null;
        }
    }

    internal User()
    {
        Username = string.Empty;
        PasswordHash = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        IndexNumber = string.Empty;
    }

    public static User Create(
        string username,
        string passwordHash,
        string firstName,
        string lastName,
        Guid facultyId,
        UserRole role,
        string? indexNumber = null
    )
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            FacultyId = facultyId,
            Role = role,
            IndexNumber = (role == UserRole.Student) ? indexNumber : null,
        };
    }

    public void UpdateDetails(
        string firstName,
        string lastName,
        Guid facultyId,
        string? indexNumber
    )
    {
        FirstName = firstName;
        LastName = lastName;
        FacultyId = facultyId;

        if (Role == UserRole.Student)
        {
            IndexNumber = indexNumber ?? string.Empty;
        }
    }

    public void ChangeStatus(UserStatus newStatus)
    {
        Status = newStatus;
    }

    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
    }
}
