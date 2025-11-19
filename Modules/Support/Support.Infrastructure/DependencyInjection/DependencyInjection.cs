using Microsoft.Extensions.DependencyInjection;
using Support.Core.Interfaces;
using Support.Application.Services;
using Support.Infrastructure.Db;

namespace Support.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSupportModule(this IServiceCollection services)
        {
            // Repozitoriji
            services.AddScoped<IRequestRepository, RequestRepository>();

            // Servisi
            services.AddScoped<IRequestService, RequestService>();

            return services;
        }
    }
}
