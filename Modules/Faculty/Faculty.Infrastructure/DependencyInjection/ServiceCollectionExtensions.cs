using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Faculty.Infrastructure.Repositories;
using Faculty.Application.Services;
using Faculty.Application.Interfaces;
using Faculty.Infrastructure.EventHandler;
using Faculty.Infrastructure.Http;

namespace Faculty.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFacultyModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ICourseService, CourseService>();

            services.AddHttpContextAccessor();
            services.AddScoped<ITenantContext, ThreadLocalTenantContext>();
            services.AddScoped<ITenantService, HttpTenantService>();
            
            services.AddScoped<StudentService>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            
            // Event handlers
            services.AddScoped<UserCreatedEventHandler>();

            services.AddDbContext<FacultyDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database")));

            return services;
        }
    }
}
