using Identity.Core.Configuration;

namespace Identity.Infrastructure.Configuration;

public static class EnvConfigProvider
{
    public static SuperAdminSettings GetSuperAdminSettings()
    {
        // TODO: TEAM KILO MIGRATION - Ensure variable names match future naming conventions
        var settings = new SuperAdminSettings
        {
            Username = Environment.GetEnvironmentVariable("SUPERADMIN_USERNAME") ?? string.Empty,
            Email = Environment.GetEnvironmentVariable("SUPERADMIN_EMAIL") ?? string.Empty,
            Password = Environment.GetEnvironmentVariable("SUPERADMIN_PASSWORD") ?? string.Empty,
            FirstName = Environment.GetEnvironmentVariable("SUPERADMIN_FIRSTNAME") ?? string.Empty,
            LastName = Environment.GetEnvironmentVariable("SUPERADMIN_LASTNAME") ?? string.Empty,
        };

        // Optional: Parse FacultyId if provided in .env, otherwise keep Guid.Empty
        var facultyIdStr = Environment.GetEnvironmentVariable("SUPERADMIN_FACULTY_ID");
        if (Guid.TryParse(facultyIdStr, out var fid))
        {
            settings.FacultyId = fid;
        }

        return settings;
    }
}