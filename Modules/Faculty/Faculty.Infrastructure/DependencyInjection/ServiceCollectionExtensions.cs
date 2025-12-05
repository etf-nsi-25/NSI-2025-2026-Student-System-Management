using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faculty.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddFacultyModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<FacultyDbContext>(options =>
				options.UseNpgsql(configuration.GetConnectionString("Database")));
			return services;
		}
	}
}
