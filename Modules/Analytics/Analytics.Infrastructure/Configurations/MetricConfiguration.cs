using Analytics.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Analytics.Infrastructure.Configurations;

public class MetricConfiguration : IEntityTypeConfiguration<Metric>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Metric> builder)
    {
        builder.ToTable("Metrics");

        builder.HasKey(m => m.Code);

        builder.Property(m => m.Code)
               .IsRequired()
               .HasMaxLength(25)
               .ValueGeneratedNever();

        builder.Property(m => m.Description)
               .IsRequired()
               .HasMaxLength(100);

    }

}
