using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Configurations
{
    public class ProgramConfiguration : IEntityTypeConfiguration<ProgramSchema>
    {
        public void Configure(EntityTypeBuilder<ProgramSchema> builder)
        {
            builder.ToTable("Programs");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Code).IsRequired().HasMaxLength(20);

            builder.HasIndex(x => new { x.DepartmentId, x.Code }).IsUnique();

            builder.HasOne(p => p.Department)
                .WithMany(d => d.Programs)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}