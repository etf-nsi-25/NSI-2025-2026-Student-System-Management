using Microsoft.Extensions.DependencyInjection;
using Support.Application.Services;
using Support.Core.Interfaces;
using Support.Core.Interfaces.Repositories;
using Support.Infrastructure.Db;
using Support.Infrastructure.Db.Repositories;
using Support.Infrastructure.Services;

namespace Support.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSupportModule(this IServiceCollection services)
        {
            // Repozitoriji
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IIssueRepository, IssueRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            // Servisi
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IDocumentPdfGenerator, DocumentPdfGenerator>();
            services.AddScoped<IIssueService, IssueService>();

            return services;
        }
    }
}