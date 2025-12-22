using Microsoft.EntityFrameworkCore;
using Analytics.Core.Entities;

namespace Analytics.Infrastructure.Db
{
    public class AnalyticsDbContext : DbContext
    {
        public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Metric> Metrics { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Opcionalno: dodatne konfiguracije ako želiš
            modelBuilder.Entity<Metric>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Value).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
            });
        }
    }
}
