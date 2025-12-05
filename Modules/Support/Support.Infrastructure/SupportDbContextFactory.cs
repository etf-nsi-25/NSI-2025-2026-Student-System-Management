using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Support.Infrastructure.Db
{
	public class SupportDbContextFactory : IDesignTimeDbContextFactory<SupportDbContext>
	{
		public SupportDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<SupportDbContext>();

			var connectionString =
				Environment.GetEnvironmentVariable("DB_CONNECTION")
				?? throw new InvalidOperationException("Environment variable DB_CONNECTION is not set.");

			optionsBuilder.UseNpgsql(connectionString);

			return new SupportDbContext(optionsBuilder.Options);
		}
	}
}
