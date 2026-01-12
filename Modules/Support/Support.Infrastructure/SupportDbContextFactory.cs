using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Support.Infrastructure.Db
{
	public class SupportDbContextFactory : IDesignTimeDbContextFactory<SupportDbContext>
	{
		public SupportDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<SupportDbContext>();

            var configurationPath = FindApplicationDirectory();

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
                    $"Connection string 'Database' not found in configuration. Searched in: {configurationPath}. " +
                    "Please ensure it's configured in appsettings.json or appsettings.Development.json");
            }

            optionsBuilder.UseNpgsql(connectionString);

			return new SupportDbContext(optionsBuilder.Options);
		}

        private static string FindApplicationDirectory()
        {
            var currentDir = Directory.GetCurrentDirectory();

            if (Directory.Exists(Path.Combine(currentDir, "Application")) ||
                File.Exists(Path.Combine(currentDir, "appsettings.json")))
            {
                if (File.Exists(Path.Combine(currentDir, "appsettings.json")))
                {
                    return currentDir;
                }

                var appDir = Path.Combine(currentDir, "Application");
                if (Directory.Exists(appDir) && File.Exists(Path.Combine(appDir, "appsettings.json")))
                {
                    return appDir;
                }
            }

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
}
