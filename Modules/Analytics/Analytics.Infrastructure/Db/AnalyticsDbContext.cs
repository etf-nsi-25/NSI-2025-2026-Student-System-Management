using Analytics.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Db;

public class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options)
    {
    }

    public DbSet<Metric> Metrics { get; set; }
    public DbSet<Stat> Stats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnalyticsDbContext).Assembly);
    }
}

