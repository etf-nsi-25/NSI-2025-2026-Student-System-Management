using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Analytics.Infrastructure.Db;

namespace Analytics.Infrastructure
{
    public class AnalyticsDbContextFactory : IDesignTimeDbContextFactory<AnalyticsDbContext>
    {
        public AnalyticsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AnalyticsDbContext>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("Database");
            optionsBuilder.UseNpgsql(connectionString);

            return new AnalyticsDbContext(optionsBuilder.Options);
        }
    }
}
