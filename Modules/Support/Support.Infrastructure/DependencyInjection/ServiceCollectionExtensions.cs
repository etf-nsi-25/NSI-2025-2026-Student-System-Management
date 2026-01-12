using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Support.Application.Services;
using Support.Core.Interfaces;
using Support.Core.Interfaces.Repositories;
using Support.Infrastructure.Db;
using Support.Infrastructure.Db.Repositories;
using Support.Infrastructure.Services;

namespace Support.Infrastructure
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddSupportModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<SupportDbContext>(options =>
				options.UseNpgsql(configuration.GetConnectionString("Database")));

			services.AddScoped<IRequestRepository, RequestRepository>();
			services.AddScoped<IIssueRepository, IssueRepository>();
			services.AddScoped<IIssueCategoryRepository, IssueCategoryRepository>();

			services.AddScoped<IRequestService, RequestService>();
			services.AddScoped<IDocumentPdfGenerator, DocumentPdfGenerator>();
			services.AddScoped<IIssueService, IssueService>();

			// Register data seeder
			services.AddHostedService<SupportDataSeeder>();

			return services;
		}
	}
}
