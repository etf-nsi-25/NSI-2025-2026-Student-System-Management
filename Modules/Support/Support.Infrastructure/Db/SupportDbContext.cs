using Microsoft.EntityFrameworkCore;
using Support.Core.Entities;

namespace Support.Infrastructure.Db
{
	public class SupportDbContext(DbContextOptions<SupportDbContext> options) : DbContext(options)
	{
		public DbSet<DocumentRequest> DocumentRequests { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(SupportDbContext).Assembly);

			base.OnModelCreating(modelBuilder);
		}
	}
}
