using Faculty.Infrastructure.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Faculty.Infrastructure.Db;

/// <summary>
/// Design-time factory for creating FacultyDbContext instances during migrations.
/// </summary>
public class FacultyDbContextFactory : IDesignTimeDbContextFactory<FacultyDbContext>
{
    /// <summary>
    /// Mock tenant service for design-time operations.
    /// </summary>
    private class DesignTimeTenantService : ITenantService
    {
        public Guid GetCurrentFacultyId()
        {
            // Return a default Guid value for design-time operations
            // This is only used during migration generation, not at runtime
            return new Guid();
        }
    }

    /// <summary>
    /// Creates a FacultyDbContext instance for design-time operations (e.g., migrations).
    /// </summary>
    /// <param name="args">Arguments provided by the design-time tools.</param>
    /// <returns>A configured FacultyDbContext instance.</returns>
    public FacultyDbContext CreateDbContext(string[] args)
    {
        // Find the Application project directory
        // When using --startup-project, the working directory is typically set to the startup project
        var configurationPath = FindApplicationDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(configurationPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("Database");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string 'Database' not found in configuration. Searched in: {configurationPath}. " +
                "Please ensure it's configured in appsettings.json or appsettings.Development.json");
        }

        // Configure DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<FacultyDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        // Create design-time tenant service
        var tenantService = new DesignTimeTenantService();

        // Create and return the DbContext
        return new FacultyDbContext(optionsBuilder.Options, tenantService);
    }

    /// <summary>
    /// Finds the Application project directory by searching common locations.
    /// </summary>
    private static string FindApplicationDirectory()
    {
        var currentDir = Directory.GetCurrentDirectory();
        
        // Try current directory first (when --startup-project is used, this should be Application)
        if (Directory.Exists(Path.Combine(currentDir, "Application")) || 
            File.Exists(Path.Combine(currentDir, "appsettings.json")))
        {
            // If we're already in Application directory or appsettings.json exists here
            if (File.Exists(Path.Combine(currentDir, "appsettings.json")))
            {
                return currentDir;
            }
            
            // If Application subdirectory exists
            var appDir = Path.Combine(currentDir, "Application");
            if (Directory.Exists(appDir) && File.Exists(Path.Combine(appDir, "appsettings.json")))
            {
                return appDir;
            }
        }

        // Search up the directory tree for Application folder
        var directory = new DirectoryInfo(currentDir);
        while (directory != null)
        {
            var applicationPath = Path.Combine(directory.FullName, "Application");
            if (Directory.Exists(applicationPath) && File.Exists(Path.Combine(applicationPath, "appsettings.json")))
            {
                return applicationPath;
            }
            
            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            $"Could not find Application project directory with appsettings.json. " +
            $"Searched from: {currentDir}");
    }
}
