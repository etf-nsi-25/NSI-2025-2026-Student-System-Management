using Identity.Application.Interfaces;
using Identity.Application.Services;
using Identity.Core.DomainServices;
using Identity.Core.Configuration;
using Identity.Core.Entities;
using Identity.Core.Interfaces.Repositories;
using Identity.Core.Interfaces.Services;
using Identity.Core.Repositories;
using Identity.Core.Services;
using Identity.Infrastructure.Db;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Services;
using Identity.Infrastructure.TOTP;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityModule(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Entity Framework
            var connectionString = configuration.GetConnectionString("Database");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("ConnectionStrings:Database configuration value is missing.");
            }

            services.AddDbContext<AuthDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.Configure<TotpSettings>(configuration.GetSection("Totp"));

            // Identity Framework
            services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>();

            // Core user-related services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IEventPublisher, EventPublisher>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // 2FA Services
            services.AddScoped<ITotpProvider, TotpProvider>(); // TOTP generator/validator
            services.AddScoped<TwoFactorDomainService>();
            services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>(); // Main 2FA service

            // Additional identity helpers
            services.AddScoped<IIdentityHasherService, IdentityHasherService>();
            services.AddScoped<ISecretEncryptionService, SecretEncryptionService>();

            return services;
        }
    }
}
