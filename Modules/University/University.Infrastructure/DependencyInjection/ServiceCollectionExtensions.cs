using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using University.Infrastructure.Db;

namespace University.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddUniversityModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<UniversityDbContext>(options =>
				options.UseNpgsql(configuration.GetConnectionString("Database")));
			return services;
		}
	}
}