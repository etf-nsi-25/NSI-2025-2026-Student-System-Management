using Identity.Application.Services;
using Identity.Core.Entities;
using Identity.Core.Repositories;
using Identity.Infrastructure.Db;
using Identity.Infrastructure.Repositories;
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
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}