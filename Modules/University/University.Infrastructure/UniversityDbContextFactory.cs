using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using University.Infrastructure.Db;

namespace University.Infrastructure
{
	public class UniversityDbContextFactory : IDesignTimeDbContextFactory<UniversityDbContext>
	{
		public UniversityDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<UniversityDbContext>();

			var connectionString =
				Environment.GetEnvironmentVariable("DB_CONNECTION")
				?? throw new InvalidOperationException("Environment variable DB_CONNECTION is not set.");

			optionsBuilder.UseNpgsql(connectionString);

			return new UniversityDbContext(optionsBuilder.Options);
		}
	}
}
