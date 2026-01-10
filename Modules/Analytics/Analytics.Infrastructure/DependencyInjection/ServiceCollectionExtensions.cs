using Analytics.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAnalyticsModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AnalyticsDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database")));

            return services;
        }
    }
}
