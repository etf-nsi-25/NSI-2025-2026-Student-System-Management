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
using Faculty.Application.Handlers;
using Common.Core.Interfaces.Repsitories;
using Common.Infrastructure.Repositories;
using MediatR;

namespace Faculty.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFacultyModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FacultyDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database")));

            services.AddScoped<DbContext>(sp => sp.GetRequiredService<FacultyDbContext>());
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
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

           

            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(GetStudentAcademicDataHandler).Assembly);
            });

            return services;
        }
    }
}
