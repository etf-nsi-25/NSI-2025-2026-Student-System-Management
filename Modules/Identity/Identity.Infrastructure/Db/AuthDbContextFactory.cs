using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Identity.Infrastructure.Db;

public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();

        var configurationPath = FindApiDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(configurationPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Database");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string 'Database' not found in configuration. Searched in: {configurationPath}.");
        }

        optionsBuilder.UseNpgsql(connectionString);

        return new AuthDbContext(optionsBuilder.Options);
    }

    private static string FindApiDirectory()
    {
        var currentDir = Directory.GetCurrentDirectory();

        var directory = new DirectoryInfo(currentDir);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "appsettings.json")))
            {
                return directory.FullName;
            }

            var apiPath = Path.Combine(directory.FullName, "Modules", "Identity", "Identity.API");
            if (Directory.Exists(apiPath) && File.Exists(Path.Combine(apiPath, "appsettings.json")))
            {
                return apiPath;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            $"Could not find Identity.API project directory with appsettings.json. Searched from: {currentDir}");
    }
}