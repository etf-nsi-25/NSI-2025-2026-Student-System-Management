using Identity.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Identity.Core.Entities; 

namespace Identity.Infrastructure.Entities; 

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid FacultyId { get; set; }
    public string? IndexNumber { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public bool ForcePasswordChange { get; set; } = false;

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}