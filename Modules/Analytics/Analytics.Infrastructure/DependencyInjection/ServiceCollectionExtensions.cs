using Analytics.Core.Interfaces;
using Analytics.Infrastructure.Db;
using Analytics.Infrastructure.Db.Seeding;
using Analytics.Infrastructure.Repositories;
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
            
            services.AddScoped<IStatRepository, StatsRepository>();
            services.AddScoped<IMetricRepository, MetricRepository>();
            services.AddScoped<AnalyticsDbInitializer>();

            return services;
        }
    }
}
