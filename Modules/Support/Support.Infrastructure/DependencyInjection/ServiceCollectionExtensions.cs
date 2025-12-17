using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Support.Application.Services;
using Support.Core.Interfaces.Repositories;
using Support.Infrastructure.Db;
using Support.Infrastructure.Db.Repositories;

namespace Support.Infrastructure
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddSupportModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<SupportDbContext>(options =>
				options.UseNpgsql(configuration.GetConnectionString("Database")));

			services.AddScoped<IIssueRepository, IssueRepository>();
			services.AddScoped<ICategoryRepository, CategoryRepository>();

			services.AddScoped<IIssueService, IssueService>();

			return services;
		}
	}
}
