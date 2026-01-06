﻿using Faculty.Core.Interfaces;
﻿using Faculty.Infrastructure.Configurations;
using Faculty.Infrastructure.Schemas;
using Faculty.Infrastructure.Http;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Db;

/// <summary>
/// Database context for the Faculty module with multi-tenancy support.
/// </summary>
public class FacultyDbContext : DbContext
{
    private readonly ITenantService _tenantService;

    public FacultyDbContext(DbContextOptions<FacultyDbContext> options, ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
    }

    private Guid CurrentFacultyId => _tenantService.GetCurrentFacultyId();

    public DbSet<TeacherSchema> Teachers { get; set; } = null!;
    public DbSet<StudentSchema> Students { get; set; } = null!;
    public DbSet<CourseSchema> Courses { get; set; } = null!;
    public DbSet<CourseAssignmentSchema> CourseAssignments { get; set; } = null!;
    public DbSet<EnrollmentSchema> Enrollments { get; set; } = null!;
    public DbSet<AssignmentSchema> Assignments { get; set; } = null!;
    public DbSet<StudentAssignmentSchema> StudentAssignments { get; set; } = null!;
    public DbSet<ExamSchema> Exams { get; set; } = null!;
    public DbSet<ExamRegistrationSchema> ExamRegistrations { get; set; } = null!;
    public DbSet<StudentExamGradeSchema> StudentExamGrades { get; set; } = null!;
    public DbSet<AttendanceSchema> Attendances { get; set; } = null!;

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyTenantInformation();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyTenantInformation();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("public");

        modelBuilder.ApplyConfiguration(new TeacherConfiguration());
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new CourseAssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new AssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new StudentAssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new ExamConfiguration());
        modelBuilder.ApplyConfiguration(new ExamRegistrationConfiguration());
        modelBuilder.ApplyConfiguration(new StudentExamGradeConfiguration());
        modelBuilder.ApplyConfiguration(new AttendanceConfiguration());

        modelBuilder.Entity<TeacherSchema>().HasQueryFilter(e => e.FacultyId == CurrentFacultyId);
        modelBuilder.Entity<StudentSchema>().HasQueryFilter(e => e.FacultyId == CurrentFacultyId);
        modelBuilder.Entity<CourseSchema>().HasQueryFilter(e => e.FacultyId == CurrentFacultyId);
        modelBuilder.Entity<CourseAssignmentSchema>().HasQueryFilter(e => e.FacultyId == CurrentFacultyId);
        modelBuilder.Entity<EnrollmentSchema>().HasQueryFilter(e => e.FacultyId == CurrentFacultyId);
        modelBuilder.Entity<AssignmentSchema>().HasQueryFilter(e => e.FacultyId == CurrentFacultyId);
        modelBuilder.Entity<StudentAssignmentSchema>().HasQueryFilter(e => e.FacultyId == CurrentFacultyId);
        modelBuilder.Entity<ExamSchema>().HasQueryFilter(e => e.FacultyId == CurrentFacultyId);
        modelBuilder.Entity<ExamRegistrationSchema>().HasQueryFilter(e => e.FacultyId == CurrentFacultyId);
        modelBuilder.Entity<StudentExamGradeSchema>().HasQueryFilter(e => e.FacultyId == CurrentFacultyId);
        modelBuilder.Entity<AttendanceSchema>().HasQueryFilter(e => e.FacultyId == CurrentFacultyId);
    }

    private void ApplyTenantInformation()
    {
        foreach (var entry in ChangeTracker.Entries<ITenantAware>())
        {
            if (entry.State == EntityState.Added && entry.Entity.FacultyId == Guid.Empty)
            {
                entry.Entity.FacultyId = CurrentFacultyId;
            }
        }
    }
    
}