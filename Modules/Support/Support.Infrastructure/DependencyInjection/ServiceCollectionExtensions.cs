using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Support.Infrastructure.Db;

namespace Support.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddSupportModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<SupportDbContext>(options =>
				options.UseNpgsql(configuration.GetConnectionString("Database")));
			return services;
		}
	}
}
