using Common.Core.Interfaces.Repsitories;
using Faculty.Application;
using Faculty.Application.Interfaces;
using Faculty.Application.Services;
using Faculty.Application.Validators;
using Faculty.Core.Entities;
using Faculty.Core.Http;
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

            // Teacher
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();

            // Assignment 
            services.AddScoped<IAssignmentService, AssignmentService>();
            services.AddScoped<IBaseRepository<Assignment>, AssignmentRepository>();

            services.AddHttpContextAccessor();
            services.AddScoped<ITenantService, HttpTenantService>();

            // Event handlers
            services.AddScoped<UserCreatedEventHandler>();

            // Db
            services.AddDbContext<FacultyDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database")));

            //AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, typeof(MappingProfile).Assembly);

            // Validators
            services.AddValidatorsFromAssemblyContaining<CreateExamRequestValidator>();

            return services;
        }
    }
}
