using Identity.Core.Enums;

namespace Identity.Core.Models;

public class TokenClaims
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public string TenantId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}