using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Support.Infrastructure.Db;
using University.Infrastructure.Db;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Http;

namespace Support.API.IntegrationTests;

public class SupportApiFactory : WebApplicationFactory<global::Support.API.Controllers.SupportController>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureServices(services =>
		{
			RemoveDbContext<SupportDbContext>(services);
			RemoveDbContext<UniversityDbContext>(services);
			RemoveDbContext<FacultyDbContext>(services);

			services.RemoveAll<ITenantService>();
			services.AddScoped<ITenantService, TestTenantService>();

			services.AddDbContext<SupportDbContext>(o => o.UseInMemoryDatabase("SupportTestDb"));
			services.AddDbContext<UniversityDbContext>(o => o.UseInMemoryDatabase("UniversityTestDb"));
			services.AddDbContext<FacultyDbContext>(o => o.UseInMemoryDatabase("FacultyTestDb"));

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = "TestScheme";
				options.DefaultChallengeScheme = "TestScheme";
			})
			.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", _ => { });
		});
	}

	private static void RemoveDbContext<T>(IServiceCollection services) where T : DbContext
	{
		services.RemoveAll<DbContextOptions<T>>();
		services.RemoveAll<T>();
	}
}