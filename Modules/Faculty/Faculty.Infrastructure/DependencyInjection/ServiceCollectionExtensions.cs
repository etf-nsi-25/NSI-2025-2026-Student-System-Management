using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Faculty.Infrastructure.Repositories;
using Faculty.Application.Services;
using Faculty.Application.Interfaces;
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
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IStudentExamRegistrationRepository, StudentExamRegistrationRepository>();
            services.AddScoped<IStudentExamRegistrationService, StudentExamRegistrationService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantService, HttpTenantService>();
            services.AddDbContext<FacultyDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database")));

            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(GetStudentAcademicDataHandler).Assembly);
            });

            return services;
        }
    }
}
