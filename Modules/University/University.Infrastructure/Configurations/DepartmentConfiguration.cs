using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<DepartmentSchema>
    {
        public void Configure(EntityTypeBuilder<DepartmentSchema> builder)
        {
            builder.ToTable("Departments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Code).IsRequired().HasMaxLength(20);

            builder.HasIndex(x => new { x.FacultyId, x.Code }).IsUnique();

            builder.HasOne(d => d.Faculty)
                .WithMany(f => f.Departments)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
