using Analytics.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Analytics.Infrastructure.Configurations;

public class StatConfiguration : IEntityTypeConfiguration<Stat>
{
       public void Configure(EntityTypeBuilder<Stat> builder)
       {
              builder.ToTable("Stats");

              builder.HasKey(m => m.Id);

              builder.HasOne(m => m.Metric)
                     .WithMany()
                     .HasForeignKey(m => m.MetricCode)
                     .OnDelete(DeleteBehavior.Restrict);

              builder.Property(m => m.Value)
                     .IsRequired()
                     .HasColumnType("jsonb")
                     .HasMaxLength(4000);


              builder.Property(m => m.Scope)
                     .HasConversion<string>()
                     .HasMaxLength(20)
                     .IsRequired();


              builder.Property(m => m.RecordedAt)
                     .IsRequired();

              builder.Property(m => m.AcademicYear)
                     .HasMaxLength(9);

              builder.HasIndex(m => new { m.Scope, m.ScopeIdentifier });
       }
}
