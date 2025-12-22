using Microsoft.EntityFrameworkCore;
using Support.Core.Entities;

namespace Support.Infrastructure.Db
{
    public class SupportDbContext(DbContextOptions<SupportDbContext> options) : DbContext(options)
    {
        public DbSet<DocumentRequest> DocumentRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SupportDbContext).Assembly);

            modelBuilder.Entity<DocumentRequest>().HasData(
                new DocumentRequest
                {
                    Id = 1,
                    UserId = "18001/2024",
                    FacultyId = 1,
                    DocumentType = "Transcript of Records",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow.AddHours(-1)
                },
                new DocumentRequest
                {
                    Id = 2,
                    UserId = "19005/2023",
                    FacultyId = 1,
                    DocumentType = "Certificate of Enrollment",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow.AddHours(-3)
                },
                new DocumentRequest
                {
                    Id = 3,
                    UserId = "21045/2024",
                    FacultyId = 2,
                    DocumentType = "Diploma Supplement",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow.AddHours(-5)
                },
                new DocumentRequest
                {
                    Id = 4,
                    UserId = "18099/2022",
                    FacultyId = 1,
                    DocumentType = "Health Insurance Form",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow.AddHours(-12)
                },
                new DocumentRequest
                {
                    Id = 5,
                    UserId = "22011/2024",
                    FacultyId = 1,
                    DocumentType = "Tax Relief Certificate",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new DocumentRequest
                {
                    Id = 6,
                    UserId = "19033/2023",
                    FacultyId = 2,
                    DocumentType = "Military Service Proof",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new DocumentRequest
                {
                    Id = 7,
                    UserId = "20088/2024",
                    FacultyId = 1,
                    DocumentType = "Public Transport Discount",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-15)
                },
                new DocumentRequest
                {
                    Id = 8,
                    UserId = "25001/2024",
                    FacultyId = 2,
                    DocumentType = "Student Visa Support",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                },
                new DocumentRequest
                {
                    Id = 9,
                    UserId = "24012/2024",
                    FacultyId = 1,
                    DocumentType = "Erasmus Application Form",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow.AddHours(-4)
                }
            );
        }
    }
}