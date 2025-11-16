using Microsoft.EntityFrameworkCore;
using Faculty.Core.Entities;

namespace Faculty.Infrastructure.Db
{
    public class FacultyDbContext : DbContext
    {
        public FacultyDbContext(DbContextOptions<FacultyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>(builder =>
            {
                builder.ToTable("faculty_Course");

                builder.HasKey(c => c.Id);

                builder.Property(c => c.CourseIdFromUniversity).IsRequired();
                builder.Property(c => c.FacultyId).IsRequired();
                builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
                builder.Property(c => c.Code).HasMaxLength(50).IsRequired();
                builder.Property(c => c.Ects).IsRequired();
                builder.Property(c => c.AcademicYear).HasMaxLength(20).IsRequired();
                builder.Property(c => c.Semester).HasMaxLength(20).IsRequired();
                builder.Property(c => c.ProfessorId).IsRequired();
            });
        }
    }
}
