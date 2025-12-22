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

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Analytics.Infrastructure.GetUniversityMetricsHandler).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(Analytics.Infrastructure.GetUniversityMetricsQuery).Assembly);
            });



            return services;
        }
    }
}
