using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Analytics.Application.Handlers;

namespace Analytics.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAnalyticsModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(GetStudentAnalyticsHandler).Assembly);
        });

        return services;
    }
}
