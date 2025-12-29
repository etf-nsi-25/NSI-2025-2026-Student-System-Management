using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using Support.Infrastructure.Db;
using University.Infrastructure.Db;
using Faculty.Infrastructure.Db;

public class SupportApiFactory : WebApplicationFactory<global::Support.API.Controllers.SupportController>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureServices(services =>
		{
			RemoveDbContext<SupportDbContext>(services);
			RemoveDbContext<UniversityDbContext>(services);
			RemoveDbContext<FacultyDbContext>(services);

			services.AddDbContext<SupportDbContext>(options => options.UseInMemoryDatabase("SupportTestDb"));
			services.AddDbContext<UniversityDbContext>(options => options.UseInMemoryDatabase("UniversityTestDb"));
			services.AddDbContext<FacultyDbContext>(options => options.UseInMemoryDatabase("FacultyTestDb"));

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = "TestScheme";
				options.DefaultChallengeScheme = "TestScheme";
			})
			.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
		});
	}

	private void RemoveDbContext<T>(IServiceCollection services) where T : DbContext
	{
		var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
		if (descriptor != null) services.Remove(descriptor);
	}
}