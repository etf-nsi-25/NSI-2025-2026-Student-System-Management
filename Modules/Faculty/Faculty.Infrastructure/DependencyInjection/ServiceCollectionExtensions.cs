using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Faculty.Infrastructure.Repositories;
using Faculty.Application.Services;
using Faculty.Application.Interfaces;
using Faculty.Infrastructure.Http;
using Faculty.Infrastructure.Http;

namespace Faculty.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFacultyModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<StudentService>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ICourseAssignmentRepository, CourseAssignmentRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IStudentExamRegistrationRepository, StudentExamRegistrationRepository>();
            services.AddScoped<IStudentExamRegistrationService, StudentExamRegistrationService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantService, HttpTenantService>();
            services.AddDbContext<FacultyDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database")));

            return services;
        }
    }
}
