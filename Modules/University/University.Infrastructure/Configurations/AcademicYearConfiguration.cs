using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Configurations
{
    public class AcademicYearConfiguration : IEntityTypeConfiguration<AcademicYear>
    {
        public void Configure(EntityTypeBuilder<AcademicYear> builder)
        {
            builder.ToTable("AcademicYears", "university");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Year)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Year).IsUnique();
        }
    }
}
