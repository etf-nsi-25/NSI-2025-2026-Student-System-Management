using Faculty.Application.Interfaces;
using Faculty.Application.Services;
using Faculty.Application.Validators;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.EventHandler;
using Faculty.Infrastructure.Http;
using Faculty.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faculty.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFacultyModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Services (NOTE: StudentService nema interfejs)
            services.AddScoped<StudentService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IStudentExamRegistrationService, StudentExamRegistrationService>();

            // Repositories
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ICourseAssignmentRepository, CourseAssignmentRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IExamRepository, ExamRepository>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IStudentExamRegistrationRepository, StudentExamRegistrationRepository>();

            services.AddScoped<FacultyDbContextSeed>();

            // Tenant
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantService, HttpTenantService>();

            // Event handlers
            services.AddScoped<UserCreatedEventHandler>();

            // Db
            services.AddDbContext<FacultyDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database")));

            // Validators
            services.AddValidatorsFromAssemblyContaining<CreateExamRequestValidator>();

            return services;
        }
    }
}
