using Identity.Application.Services;
using Identity.Core.Interfaces;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration config)
    {
        var cs = config.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(cs, b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Services (adjust to your layering preference)
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}