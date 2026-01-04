using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Support.Core.Entities;
using Support.Infrastructure.Db;

namespace Support.Infrastructure.Services
{
	public class SupportDataSeeder : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<SupportDataSeeder> _logger;

		public SupportDataSeeder(IServiceProvider serviceProvider, ILogger<SupportDataSeeder> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			using var scope = _serviceProvider.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<SupportDbContext>();

			try
			{
				// Check if categories already exist
				var existingCount = await context.IssueCategories.CountAsync(cancellationToken);
				
				if (existingCount == 0)
				{
					_logger.LogInformation("Seeding issue categories...");

					var categories = new List<IssueCategory>
					{
						new IssueCategory { Title = "Academic support", Priority = 1 },
						new IssueCategory { Title = "Technical issues", Priority = 2 },
						new IssueCategory { Title = "Administrative help", Priority = 3 },
						new IssueCategory { Title = "Account & Security", Priority = 4 }
					};

					await context.IssueCategories.AddRangeAsync(categories, cancellationToken);
					await context.SaveChangesAsync(cancellationToken);

					_logger.LogInformation("Issue categories seeded successfully.");
				}
				else
				{
					_logger.LogInformation("Issue categories already exist. Skipping seeding.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error seeding issue categories.");
			}
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}
