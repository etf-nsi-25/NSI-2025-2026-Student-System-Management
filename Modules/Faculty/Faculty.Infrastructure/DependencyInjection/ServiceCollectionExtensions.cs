using Faculty.Application.Interfaces;
using Faculty.Application.Services;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.EventHandler;
using Faculty.Infrastructure.Http;
using Faculty.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Faculty.Infrastructure.Repositories;
using Faculty.Infrastructure.Http;

namespace Faculty.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFacultyModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IStudentExamRegistrationRepository, StudentExamRegistrationRepository>();
            services.AddScoped<IStudentExamRegistrationService, StudentExamRegistrationService>();
            services.AddScoped<StudentService>();
            services.AddScoped<IStudentRepository, StudentRepository>();

            services.AddHttpContextAccessor();
            services.AddScoped<ITenantService, HttpTenantService>();
            
            // Event handlers
            services.AddScoped<UserCreatedEventHandler>();

            services.AddDbContext<FacultyDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database")));

            return services;
        }
    }
}
