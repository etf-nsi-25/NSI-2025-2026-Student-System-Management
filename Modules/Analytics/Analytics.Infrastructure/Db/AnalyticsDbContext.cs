using Analytics.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Db;

public class AnalyticsDbContext : DbContext
{
	public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options) { }

	public DbSet<Metric> Metrics => Set<Metric>();
	public DbSet<Report> Reports => Set<Report>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("analytics");

		modelBuilder.Entity<Metric>(b =>
		{
			b.ToTable("Metric");
			b.HasKey(x => x.Id);

			b.Property(x => x.Name).IsRequired();
			b.Property(x => x.Value).IsRequired();
			b.Property(x => x.UserId).IsRequired();

			b.HasIndex(x => new { x.FacultyId, x.UserId, x.Name, x.Timestamp });
		});

		modelBuilder.Entity<Report>(b =>
		{
			b.ToTable("Report");
			b.HasKey(x => x.Id);

			b.Property(x => x.Name).IsRequired();
			b.Property(x => x.GeneratedAt).IsRequired();
		});
	}
}
