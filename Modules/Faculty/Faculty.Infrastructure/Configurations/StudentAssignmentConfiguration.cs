using Faculty.Infrastructure.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faculty.Infrastructure.Configurations;

public class StudentAssignmentConfiguration : IEntityTypeConfiguration<StudentAssignmentSchema>
{
    public void Configure(EntityTypeBuilder<StudentAssignmentSchema> builder)
    {
        builder.ToTable("StudentAssignment");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.FacultyId).IsRequired();
        builder.Property(e => e.StudentId).IsRequired();
        builder.Property(e => e.AssignmentId).IsRequired();
        builder.Property(e => e.SubmissionDate);
        builder.Property(e => e.Points);
        builder.Property(e => e.Grade);
        builder.Property(e => e.Feedback).HasColumnType("text");
        builder.Property(e => e.SubmissionUrl).HasMaxLength(500);
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt);

        builder.HasOne(e => e.Student)
            .WithMany(s => s.StudentAssignments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Assignment)
            .WithMany(a => a.StudentAssignments)
            .HasForeignKey(e => e.AssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.FacultyId);
        builder.HasIndex(e => new { e.StudentId, e.AssignmentId });
    }
}
