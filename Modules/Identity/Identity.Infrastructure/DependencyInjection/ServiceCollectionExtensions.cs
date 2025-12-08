using Identity.Application.Services;
using Identity.Core.Entities;
using Identity.Core.Interfaces.Repositories;
using Identity.Core.Interfaces.Services;
using Identity.Core.Repositories;
using Identity.Core.Services;
using Identity.Infrastructure.Db;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Services;
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
            IConfiguration configuration)
        {
            // Entity Framework   
            services.AddDbContext<AuthDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database"))
            );

            // Identity Framework
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>();

            // Register services
            
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IIdentityHasherService, IdentityHasherService>();
            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            


            return services;
        }
    }
}