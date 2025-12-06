using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Entities;

public class ApplicationUser : IdentityUser
{
    public string? TwoFactorSecretEncrypted { get; set; }
    public string? TwoFactorSecretPending { get; set; }
}
