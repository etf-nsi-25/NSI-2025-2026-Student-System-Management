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

        public DbSet<AcademicYearSchema> AcademicYears => Set<AcademicYearSchema>();
        public DbSet<FacultySchema> Faculties => Set<FacultySchema>();
        public DbSet<DepartmentSchema> Departments => Set<DepartmentSchema>();
        public DbSet<ProgramSchema> Programs => Set<ProgramSchema>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UniversityDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}