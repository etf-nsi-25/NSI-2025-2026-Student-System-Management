using Faculty.Core.Interfaces;
using Faculty.Core.Services;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faculty.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Faculty module services to the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFacultyModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register HttpContextAccessor (required for HttpTenantService)
            services.AddHttpContextAccessor();

            // Register tenant service
            services.AddScoped<ITenantService, HttpTenantService>();

            // Register Entity Framework DbContext
            services.AddDbContext<FacultyDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory_Faculty", "faculty")
                )
            );

            return services;
        }
    }
}
