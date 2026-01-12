using EventBus.Core;
using EventBus.MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            cfg.Lifetime = ServiceLifetime.Scoped;
        });
            
        services.AddScoped<IEventBus, MediatREventBus>();
        return services;
    }
}
