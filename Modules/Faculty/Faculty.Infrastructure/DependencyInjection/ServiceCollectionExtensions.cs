using Faculty.Core.Interfaces;
using Faculty.Core.Services;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Faculty.Infrastructure.Repositories;
using Faculty.Application.Services;
using Faculty.Application.Interfaces;
using FluentValidation;
using Faculty.Application.Validators;

namespace Faculty.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFacultyModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IExamRepository, ExamRepository>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantService, HttpTenantService>();
            services.AddDbContext<FacultyDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database")));

            // Add FluentValidation
            services.AddValidatorsFromAssemblyContaining<CreateExamRequestValidator>();

            return services;
        }
    }
}
