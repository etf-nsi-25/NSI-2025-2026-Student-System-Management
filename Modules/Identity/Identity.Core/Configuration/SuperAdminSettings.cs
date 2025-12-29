namespace Identity.Core.Configuration;

public class SuperAdminSettings
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    // TODO: TEAM KILO MIGRATION - Check if Superadmin requires a specific FacultyId
    // For now, we default to Empty for global admins.
    public Guid FacultyId { get; set; } = Guid.Empty; 

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Email) &&
               !string.IsNullOrWhiteSpace(Password);
    }
}