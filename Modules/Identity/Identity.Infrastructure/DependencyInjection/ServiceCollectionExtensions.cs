using Identity.Application.Interfaces;
using Identity.Application.Security;
using Identity.Application.Services;
using Identity.Core.Configuration;
using Identity.Core.Interfaces.Repositories;
using Identity.Core.Interfaces.Services;
using Identity.Infrastructure.Db;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Load environment variables
            var env = DotNetEnv.Env.TraversePath().Load();
            if (env == null || !env.Any())
            {
                DotNetEnv.Env.TraversePath().Load(".env.example");
            }

            // Data Protection (Required for Token Providers)
            services.AddDataProtection();

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Entity Framework - only add if not already configured (for tests)
            if (!services.Any(d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>)))
            {
                services.AddDbContext<AuthDbContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("Database")));
            }

            // Identity Framework - only add if not already configured (for tests)
            if (!services.Any(d => d.ServiceType == typeof(IUserStore<ApplicationUser>)))
            {
                services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<AuthDbContext>()
                    .AddDefaultTokenProviders();
            }

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();
            services.AddSingleton<ITwoFactorLoginSessionStore, TwoFactorLoginSessionStore>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            
            services.AddScoped<IUserNotifierService, UserNotifierService>();

            services.AddHostedService<IdentityStartupService>();

            JwtSettings jwtSettings = new JwtSettings();
            configuration.Bind("JwtSettings", jwtSettings);

            // Fallback for test environment if signing key is not provided
            if (string.IsNullOrEmpty(jwtSettings.SigningKey))
            {
                jwtSettings.SigningKey = Convert.ToBase64String(new byte[32]);
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    var keyBytes = Convert.FromBase64String(jwtSettings.SigningKey);

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}
