using Identity.Application.Services;
using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Identity.Infrastructure.Entities;
using Identity.Core.Repositories;
using Identity.Core.DomainServices;
using Identity.Infrastructure.Db;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.TOTP;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Identity.Infrastructure.Services;
using Identity.Core.Services;


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
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IEventPublisher, EventPublisher>(); 

            services.AddScoped<ITotpProvider, TotpProvider>();
            services.AddScoped<TwoFactorDomainService>();
            services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();

            services.AddScoped<IIdentityHasherService, IdentityHasherService>();
            return services;
        }
    }
}