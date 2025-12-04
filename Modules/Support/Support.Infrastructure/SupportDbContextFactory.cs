using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Support.Infrastructure.Db;

namespace Support.Infrastructure.Db
{
	public class SupportDbContextFactory : IDesignTimeDbContextFactory<SupportDbContext>
	{
		public SupportDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<SupportDbContext>();
			optionsBuilder.UseNpgsql("Host=localhost;Database=design_time_db;Username=postgres;Password=password");

			return new SupportDbContext(optionsBuilder.Options);
		}
	}
}