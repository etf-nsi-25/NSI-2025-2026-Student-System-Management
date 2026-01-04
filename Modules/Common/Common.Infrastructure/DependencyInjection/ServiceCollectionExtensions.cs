using Common.Core.Tenant;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonModule(this IServiceCollection services)
        {
            services.AddScoped<IScopedTenantContext, ThreadLocalScopedTenantContext>();

            return services;
        }
    }
}
