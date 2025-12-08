using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using University.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using University.Core.Interfaces;
using University.Infrastructure.Repositories;

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

            services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
            services.AddScoped<IFacultyRepository, FacultyRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IProgramRepository, ProgramRepository>();

            return services;
        }
    }
}