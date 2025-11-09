using Faculty.Application.Services;
using Faculty.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Faculty.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFacultyApplication(this IServiceCollection services)
        {
            services.AddScoped<IFacultyService, FacultyService>();
            return services;
        }
    }
}