using Microsoft.Extensions.DependencyInjection;
using University.Application.Interfaces.Services;
using University.Application.Services;

namespace University.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUniversityModule(this IServiceCollection services)
        {
            services.AddScoped<IFacultyService, FacultyService>();
            return services;
        }
    }
}
