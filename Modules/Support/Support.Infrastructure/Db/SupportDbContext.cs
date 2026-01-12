using Microsoft.EntityFrameworkCore;
using Support.Core.Entities;

namespace Support.Infrastructure.Db
{
	public class SupportDbContext(DbContextOptions<SupportDbContext> options) : DbContext(options)
	{
		public DbSet<DocumentRequest> DocumentRequests { get; set; } = null!;
		public DbSet<EnrollmentRequest> EnrollmentRequests { get; set; } = null!;
		public DbSet<Issue> Issues { get; set; } = null!;
		public DbSet<IssueCategory> IssueCategories { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(SupportDbContext).Assembly);

			base.OnModelCreating(modelBuilder);
		}
	}
}
