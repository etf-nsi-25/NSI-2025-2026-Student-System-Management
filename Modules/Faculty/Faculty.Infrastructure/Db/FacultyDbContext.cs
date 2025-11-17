using Faculty.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Db
{
    public class FacultyDbContext : DbContext
    {
        public FacultyDbContext(DbContextOptions<FacultyDbContext> options)
            : base(options)
        {
        }

        public DbSet<FacultyCourse> FacultyCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FacultyCourse>()
                .ToTable("faculty_Course");

            base.OnModelCreating(modelBuilder);
        }
    }
}
