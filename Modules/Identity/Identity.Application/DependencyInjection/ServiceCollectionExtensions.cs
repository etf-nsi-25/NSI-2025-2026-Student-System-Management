using Identity.Application.Services;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application.DependencyInjection;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>(); 
        return services;
    }
}