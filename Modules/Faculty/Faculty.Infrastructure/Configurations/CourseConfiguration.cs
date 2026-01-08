using Faculty.Infrastructure.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faculty.Infrastructure.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<CourseSchema>
{
    public void Configure(EntityTypeBuilder<CourseSchema> builder)
    {
        builder.ToTable("Course");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.FacultyId).IsRequired();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Code).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Type).IsRequired().HasConversion<string>();
        builder.Property(e => e.ProgramId).HasMaxLength(50);
        builder.Property(e => e.ECTS);
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt);

        builder.HasIndex(e => e.FacultyId);
        builder.HasIndex(e => e.Code);
    }
}