using Identity.Infrastructure.Db;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EventBus.Core;
using Moq;
using Common.Core.Tenant;
using Identity.Core.Configuration;

namespace Identity.IntegrationTests
{
    public class IdentityApiFactory : WebApplicationFactory<Program>
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
            builder.UseEnvironment("Development");


            builder.UseSetting("JwtSettings:SigningKey", "VGVzdFNpZ25pbmdLZXkxMjM0NTY3ODkwMTIzNDU2Nzg=");
            builder.UseSetting("JwtSettings:Issuer", "IntegrationTest");
            builder.UseSetting("JwtSettings:Audience", "IntegrationTest");

            builder.ConfigureServices((context, services) =>
            {
                services.Configure<JwtSettings>(options =>
                {
                    options.Issuer = "IntegrationTest";
                    options.Audience = "IntegrationTest";
                    options.SigningKey = "VGVzdFNpZ25pbmdLZXkxMjM0NTY3ODkwMTIzNDU2Nzg=";
                });

                var tenantContextMock = new Mock<IScopedTenantContext>();
                tenantContextMock.Setup(x => x.Use(It.IsAny<Guid>()))
                    .Returns(new Mock<IDisposable>().Object);
                services.AddScoped<IScopedTenantContext>(_ => tenantContextMock.Object);

                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));
                if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IdentityIntegrationTestDb");
                    options.UseInternalServiceProvider(_efServiceProvider);
                });

                services.AddScoped<IEventBus, TestEventBus>();
            });
        }

        public class TestEventBus : IEventBus
        {
            public Task Dispatch(IEvent domainEvent, CancellationToken ct = default) => Task.CompletedTask;
            public Task Dispatch(IEvent domainEvent, Guid tenantId, CancellationToken ct = default) => Task.CompletedTask;
        }
    }
}