using Faculty.Infrastructure.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faculty.Infrastructure.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<StudentSchema>
{
    public void Configure(EntityTypeBuilder<StudentSchema> builder)
    {
        builder.ToTable("Student");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.FacultyId).IsRequired();
        builder.Property(e => e.UserId).IsRequired().HasMaxLength(450);
        builder.Property(e => e.IndexNumber).IsRequired().HasMaxLength(50);
        builder.Property(e => e.FirstName).HasMaxLength(100);
        builder.Property(e => e.LastName).HasMaxLength(100);
        builder.Property(e => e.EnrollmentDate);
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt);

        builder.HasIndex(e => e.FacultyId);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.IndexNumber);
    }
}
