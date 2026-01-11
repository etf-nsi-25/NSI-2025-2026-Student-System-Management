using Faculty.Infrastructure.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faculty.Infrastructure.Configurations;

public class StudentExamGradeConfiguration : IEntityTypeConfiguration<StudentExamGradeSchema>
{
    public void Configure(EntityTypeBuilder<StudentExamGradeSchema> builder)
    {
        builder.ToTable("StudentExamGrade");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.FacultyId).IsRequired();
        builder.Property(e => e.StudentId).IsRequired();
        builder.Property(e => e.ExamId).IsRequired();
        builder.Property(e => e.Passed);
        builder.Property(e => e.Points);
        builder.Property(e => e.URL).HasMaxLength(500);
        builder.Property(e => e.DateRecorded);
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt);

        builder.HasOne(e => e.Student)
            .WithMany(s => s.StudentExamGrades)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Exam)
            .WithMany(ex => ex.StudentExamGrades)
            .HasForeignKey(e => e.ExamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.FacultyId);
        builder.HasIndex(e => new { e.StudentId, e.ExamId });
    }
}
