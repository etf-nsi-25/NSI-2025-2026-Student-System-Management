using Faculty.Infrastructure.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faculty.Infrastructure.Configurations;

public class CourseAssignmentConfiguration : IEntityTypeConfiguration<CourseAssignmentSchema>
{
    public void Configure(EntityTypeBuilder<CourseAssignmentSchema> builder)
    {
        builder.ToTable("CourseAssignment");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.FacultyId).IsRequired();
        builder.Property(e => e.TeacherId).IsRequired();
        builder.Property(e => e.CourseId).IsRequired();
        builder.Property(e => e.Role).HasMaxLength(50);
        builder.Property(e => e.AcademicYearId).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt);

        builder.HasOne(e => e.Teacher)
            .WithMany(t => t.CourseAssignments)
            .HasForeignKey(e => e.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Course)
            .WithMany(c => c.CourseAssignments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.FacultyId);
        builder.HasIndex(e => new { e.TeacherId, e.CourseId });
    }
}
