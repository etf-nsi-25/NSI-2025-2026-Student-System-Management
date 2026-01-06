using Identity.Infrastructure.Db;
using Identity.API.Controllers;
using Identity.Infrastructure.Entities;
using Identity.Core.Interfaces.Repositories;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EventBus.Core;
using Moq;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

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
                    {"JwtSettings:SigningKey", "VGVzdFNpZ25pbmdLZXkxMjM0NTY3ODkwMTIzNDU2Nzg="}, 
                    {"JwtSettings:AccessTokenExpirationMinutes", "60"},
                    {"JwtSettings:RefreshTokenExpirationDays", "7"}
                });
            });

            builder.ConfigureTestServices(services =>
            {
                RemoveService<DbContextOptions<AuthDbContext>>(services);
                RemoveService<AuthDbContext>(services);

                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IdentityIntegrationTestDb");
                    options.UseInternalServiceProvider(_efServiceProvider);
                });

                var refreshTokenRepoMock = new Moq.Mock<IRefreshTokenRepository>();
                
                refreshTokenRepoMock
                    .Setup(x => x.AddAsync(Moq.It.IsAny<RefreshToken>(), Moq.It.IsAny<CancellationToken>()))
                    .ReturnsAsync((RefreshToken token, CancellationToken ct) => token);
                
                RemoveService<IRefreshTokenRepository>(services);
                services.AddScoped<IRefreshTokenRepository>(_ => refreshTokenRepoMock.Object);

                services.AddScoped<IEventBus, TestEventBus>();

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                }
            });
        }

        private void RemoveService<T>(IServiceCollection services)
        {
            var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(T));
            if (serviceDescriptor != null) services.Remove(serviceDescriptor);
        }

        public class TestEventBus : IEventBus
        {
            public Task Dispatch(IEvent domainEvent, CancellationToken ct = default) => Task.CompletedTask;
            public Task Dispatch(IEvent domainEvent, Guid tenantId, CancellationToken ct = default) => Task.CompletedTask;
        }
    }
}