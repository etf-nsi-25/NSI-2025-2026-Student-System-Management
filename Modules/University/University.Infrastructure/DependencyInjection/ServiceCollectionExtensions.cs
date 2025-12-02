using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using University.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace University.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUniversityModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Entity Framework
            services.AddDbContext<UniversityDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsql =>
                    {
                        // Custom migration history table in "university" schema
                        npgsql.MigrationsHistoryTable("__EFMigrationsHistory_University", "university");
                    }
                )
            );

            // TODO: Register University-specific services or repositories here
            // Example:
            // services.AddScoped<IStudentRepository, StudentRepository>();

            return services;
        }
    }
}
