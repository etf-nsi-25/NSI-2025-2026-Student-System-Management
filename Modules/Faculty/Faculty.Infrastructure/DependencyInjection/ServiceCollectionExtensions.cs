using Faculty.Infrastructure.Db;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faculty.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFacultyInfrastructure(this IServiceCollection services, string connectionString)
        {
            // Register DbContext
            services.AddDbContext<FacultyDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });
            
            // Register repositories
            services.AddScoped<IFacultyRepository, FacultyRepository>();
            
            return services;
        }
    }
}