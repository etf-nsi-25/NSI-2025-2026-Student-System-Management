using Faculty.Infrastructure.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faculty.Infrastructure.Configurations;

public class ExamRegistrationConfiguration : IEntityTypeConfiguration<ExamRegistrationSchema>
{
    public void Configure(EntityTypeBuilder<ExamRegistrationSchema> builder)
    {
        builder.ToTable("ExamRegistration");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.FacultyId).IsRequired();
        builder.Property(e => e.StudentId).IsRequired();
        builder.Property(e => e.ExamId).IsRequired();
        builder.Property(e => e.RegistrationDate).IsRequired();
        builder.Property(e => e.Status).IsRequired().HasMaxLength(50);
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt);

        builder.HasOne(e => e.Student)
            .WithMany(s => s.ExamRegistrations)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Exam)
            .WithMany(ex => ex.ExamRegistrations)
            .HasForeignKey(e => e.ExamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.FacultyId);
        builder.HasIndex(e => new { e.StudentId, e.ExamId });
    }
}
