using Identity.Core.Configuration;

namespace Identity.Infrastructure.Configuration;

public static class EnvConfigProvider
{
    public static SuperAdminSettings GetSuperAdminSettings()
    {
        var settings = new SuperAdminSettings
        {
            Username = Environment.GetEnvironmentVariable("SUPERADMIN_USERNAME") ?? string.Empty,
            Email = Environment.GetEnvironmentVariable("SUPERADMIN_EMAIL") ?? string.Empty,
            Password = Environment.GetEnvironmentVariable("SUPERADMIN_PASSWORD") ?? string.Empty,
            FirstName = Environment.GetEnvironmentVariable("SUPERADMIN_FIRSTNAME") ?? string.Empty,
            LastName = Environment.GetEnvironmentVariable("SUPERADMIN_LASTNAME") ?? string.Empty,
        };

        return settings;
    }
}