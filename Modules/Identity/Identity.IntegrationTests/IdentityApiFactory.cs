using Identity.Infrastructure.Db;
using Identity.API.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EventBus.Core;

namespace Identity.IntegrationTests
{
    public class IdentityApiFactory : WebApplicationFactory<AuthController>
    {
        private readonly IServiceProvider _efServiceProvider;

        public IdentityApiFactory()
        {
            _efServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"JwtSettings:Issuer", "IntegrationTest"},
                    {"JwtSettings:Audience", "IntegrationTest"},
                    {"JwtSettings:SigningKey", Convert.ToBase64String(new byte[32])}
                });
            });

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<AuthDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var contextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(AuthDbContext));
                if (contextDescriptor != null)
                {
                    services.Remove(contextDescriptor);
                }

                var optionsDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions));
                if (optionsDescriptor != null)
                {
                    services.Remove(optionsDescriptor);
                }

                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IdentityIntegrationTestDb");
                    options.UseInternalServiceProvider(_efServiceProvider);
                });

                services.AddScoped<IEventBus, TestEventBus>();

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AuthDbContext>();
                    db.Database.EnsureCreated();
                }

                services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<AuthDbContext>()
                    .AddDefaultTokenProviders();
            });
        }

        // A simple test event bus that does nothing, sufficient for integration tests
        public class TestEventBus : IEventBus
        {
            public Task Dispatch(IEvent domainEvent, CancellationToken ct = default)
            {
                return Task.CompletedTask;
            }

            public Task Dispatch(IEvent domainEvent, Guid tenantId, CancellationToken ct = default)
            {
                return Task.CompletedTask;
            }
        }
    }
}
