using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Db
{
    public class UniversityDbContext : DbContext
    {
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
        : base(options)
        {
        }

        public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
        public DbSet<Faculty> Faculties => Set<Faculty>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Program> Programs => Set<Program>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);
        }
    }
}