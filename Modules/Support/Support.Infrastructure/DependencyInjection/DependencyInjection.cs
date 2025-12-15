using Microsoft.Extensions.DependencyInjection;
using Support.Application.Services;
using Support.Core.Interfaces;
using Support.Infrastructure.Db;
using Support.Infrastructure.Services;

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
            services.AddScoped<IDocumentPdfGenerator, DocumentPdfGenerator>();

            return services;
        }
    }
}