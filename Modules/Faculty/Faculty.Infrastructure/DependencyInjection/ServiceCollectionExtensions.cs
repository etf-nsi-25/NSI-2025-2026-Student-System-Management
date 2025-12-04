using Faculty.Core.Interfaces;
using Faculty.Core.Services;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faculty.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFacultyModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ICourseService, CourseService>();
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