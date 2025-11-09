using Microsoft.EntityFrameworkCore;
using Faculty.Core.Entities;
using Faculty.Core.Enums;

namespace Faculty.Infrastructure.Db
{
    public class FacultyDbContext : DbContext
    {
        public DbSet<Faculty.Core.Entities.Faculty> Faculties { get; set; }
        
        public FacultyDbContext(DbContextOptions<FacultyDbContext> options) 
            : base(options)
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Enable sensitive data logging for development only
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure unique constraints
            modelBuilder.Entity<Faculty.Core.Entities.Faculty>()
                .HasIndex(f => f.Name)
                .IsUnique();
            
            modelBuilder.Entity<Faculty.Core.Entities.Faculty>()
                .HasIndex(f => f.Abbreviation)
                .IsUnique();
            
            // Seed initial data
            var now = DateTime.UtcNow;
            modelBuilder.Entity<Faculty.Core.Entities.Faculty>().HasData(
                new Faculty.Core.Entities.Faculty
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Name = "Faculty of Computer Science",
                    Abbreviation = "FCS",
                    Description = "Leading technology and innovation education",
                    Status = FacultyStatus.Active,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Faculty.Core.Entities.Faculty
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Name = "Faculty of Engineering",
                    Abbreviation = "FENG",
                    Description = "Comprehensive engineering programs",
                    Status = FacultyStatus.Active,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Faculty.Core.Entities.Faculty
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                    Name = "Faculty of Business",
                    Abbreviation = "FBUS",
                    Description = "Business administration and management",
                    Status = FacultyStatus.Inactive,
                    CreatedAt = now,
                    UpdatedAt = now
                }
            );
        }
    }
}