using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Configurations
{
    public class AcademicYearConfiguration : IEntityTypeConfiguration<AcademicYearSchema>
    {
        public void Configure(EntityTypeBuilder<AcademicYearSchema> builder)
        {
            builder.ToTable("AcademicYears");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Year)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Year).IsUnique();
        }
    }
}
