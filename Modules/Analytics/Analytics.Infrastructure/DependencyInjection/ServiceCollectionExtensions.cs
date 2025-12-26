using Analytics.Application.Services;
using Analytics.Core.Interfaces;
using Analytics.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAnalyticsModule(this IServiceCollection services)
        {
			services.AddScoped<IStudentAnalyticsRepository, StudentAnalyticsRepository>();

			services.AddScoped<IStudentAnalyticsService, StudentAnalyticsService>();

			return services;
        }
    }
}
