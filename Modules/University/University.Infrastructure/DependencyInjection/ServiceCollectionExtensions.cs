using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using University.Application.Interfaces;
using University.Application.Services;
using University.Core.Interfaces;
using University.Infrastructure.Db;
using University.Infrastructure.Repositories;

namespace University.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUniversityModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Entity Framework
            services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IFacultyRepository, FacultyRepository>();
            services.AddScoped<IProgramRepository, ProgramRepository>();
            services.AddDbContext<UniversityDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database")));
            services.AddScoped<UniversityDbInitializier>();

            // Application services
            services.AddScoped<IFacultyService, FacultyService>();
            
            return services;
        }
    }
}