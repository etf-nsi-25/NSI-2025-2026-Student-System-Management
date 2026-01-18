using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Configurations
{
    public class FacultyConfiguration : IEntityTypeConfiguration<FacultySchema>
    {
        public void Configure(EntityTypeBuilder<FacultySchema> builder)
        {
            builder.ToTable("Faculties");
            builder.HasKey(x => x.Id);
            builder.Property(f => f.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Address).HasMaxLength(200);
            builder.Property(x => x.Code).IsRequired().HasMaxLength(20);

            builder.HasIndex(x => x.Code).IsUnique();
        }
    }
}
