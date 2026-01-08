using Faculty.Infrastructure.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faculty.Infrastructure.Configurations;

public class ExamConfiguration : IEntityTypeConfiguration<ExamSchema>
{
    public void Configure(EntityTypeBuilder<ExamSchema> builder)
    {
        builder.ToTable("Exam");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.FacultyId).IsRequired();
        builder.Property(e => e.CourseId).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(200);
        builder.Property(e => e.Location).HasMaxLength(200);
        builder.Property(e => e.ExamType).IsRequired().HasMaxLength(50);
        builder.Property(e => e.ExamDate);
        builder.Property(e => e.RegDeadline);
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt);

        builder.HasOne(e => e.Course)
            .WithMany(c => c.Exams)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.FacultyId);
        builder.HasIndex(e => e.CourseId);
    }
}
