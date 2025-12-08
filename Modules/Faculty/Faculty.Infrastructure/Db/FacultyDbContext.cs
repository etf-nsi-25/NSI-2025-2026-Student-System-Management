using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Db;

/// <summary>
/// Database context for the Faculty module with multi-tenancy support.
/// </summary>
public class FacultyDbContext : DbContext
{
    private readonly ITenantService _tenantService;
    private readonly Guid _currentFacultyId;

    public FacultyDbContext(DbContextOptions<FacultyDbContext> options, ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
        _currentFacultyId = _tenantService.GetCurrentFacultyId();
    }

    /// <summary>
    /// Gets the current Faculty ID that was resolved during context instantiation.
    /// This value is used in query filters and can be properly translated to SQL.
    /// </summary>
    private Guid CurrentFacultyId => _currentFacultyId;

    // DbSets
    public DbSet<Teacher> Teachers { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<CourseAssignment> CourseAssignments { get; set; } = null!;
    public DbSet<Enrollment> Enrollments { get; set; } = null!;
    public DbSet<Assignment> Assignments { get; set; } = null!;
    public DbSet<StudentAssignment> StudentAssignments { get; set; } = null!;
    public DbSet<Exam> Exams { get; set; } = null!;
    public DbSet<ExamRegistration> ExamRegistrations { get; set; } = null!;
    public DbSet<StudentExamGrade> StudentExamGrades { get; set; } = null!;
    public DbSet<Attendance> Attendances { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema to "faculty"
        modelBuilder.HasDefaultSchema("faculty");

        // Configure entities with Fluent API
        ConfigureTeacher(modelBuilder);
        ConfigureStudent(modelBuilder);
        ConfigureCourse(modelBuilder);
        ConfigureCourseAssignment(modelBuilder);
        ConfigureEnrollment(modelBuilder);
        ConfigureAssignment(modelBuilder);
        ConfigureStudentAssignment(modelBuilder);
        ConfigureExam(modelBuilder);
        ConfigureExamRegistration(modelBuilder);
        ConfigureStudentExamGrade(modelBuilder);
        ConfigureAttendance(modelBuilder);
    }

    private void ConfigureTeacher(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teacher");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FacultyId).IsRequired();
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Office).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);

            // Global query filter for multi-tenancy
            // CurrentFacultyId is resolved during context instantiation and can be translated to SQL
            entity.HasQueryFilter(e => e.FacultyId == CurrentFacultyId);

            // Indexes
            entity.HasIndex(e => e.FacultyId);
            entity.HasIndex(e => e.UserId);
        });
    }

    private void ConfigureStudent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Student");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FacultyId).IsRequired();
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.IndexNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.EnrollmentDate);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);

            // Global query filter for multi-tenancy
            // The filter is evaluated at query time, ensuring tenant isolation
            entity.HasQueryFilter(e => e.FacultyId == CurrentFacultyId);

            // Indexes
            entity.HasIndex(e => e.FacultyId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IndexNumber);
        });
    }

    private void ConfigureCourse(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FacultyId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Type).IsRequired().HasConversion<string>();
            entity.Property(e => e.ProgramId).HasMaxLength(50);
            entity.Property(e => e.ECTS);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);

            // Global query filter for multi-tenancy
            // The filter is evaluated at query time, ensuring tenant isolation
            entity.HasQueryFilter(e => e.FacultyId == CurrentFacultyId);

            // Indexes
            entity.HasIndex(e => e.FacultyId);
            entity.HasIndex(e => e.Code);
        });
    }

    private void ConfigureCourseAssignment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CourseAssignment>(entity =>
        {
            entity.ToTable("CourseAssignment");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FacultyId).IsRequired();
            entity.Property(e => e.TeacherId).IsRequired();
            entity.Property(e => e.CourseId).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.AcademicYearId).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);

            // Global query filter for multi-tenancy
            // The filter is evaluated at query time, ensuring tenant isolation
            entity.HasQueryFilter(e => e.FacultyId == CurrentFacultyId);

            // Relationships
            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.CourseAssignments)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.CourseAssignments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.FacultyId);
            entity.HasIndex(e => new { e.TeacherId, e.CourseId });
        });
    }

    private void ConfigureEnrollment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("Enrollment");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FacultyId).IsRequired();
            entity.Property(e => e.StudentId).IsRequired();
            entity.Property(e => e.CourseId).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Grade);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);

            // Global query filter for multi-tenancy
            // The filter is evaluated at query time, ensuring tenant isolation
            entity.HasQueryFilter(e => e.FacultyId == CurrentFacultyId);

            // Relationships
            entity.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.FacultyId);
            entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
        });
    }

    private void ConfigureAssignment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("Assignment");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FacultyId).IsRequired();
            entity.Property(e => e.CourseId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.DueDate);
            entity.Property(e => e.MaxPoints);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);

            // Global query filter for multi-tenancy
            // The filter is evaluated at query time, ensuring tenant isolation
            entity.HasQueryFilter(e => e.FacultyId == CurrentFacultyId);

            // Relationships
            entity.HasOne(e => e.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.FacultyId);
            entity.HasIndex(e => e.CourseId);
        });
    }

    private void ConfigureStudentAssignment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentAssignment>(entity =>
        {
            entity.ToTable("StudentAssignment");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FacultyId).IsRequired();
            entity.Property(e => e.StudentId).IsRequired();
            entity.Property(e => e.AssignmentId).IsRequired();
            entity.Property(e => e.SubmissionDate);
            entity.Property(e => e.Points);
            entity.Property(e => e.Grade);
            entity.Property(e => e.Feedback).HasColumnType("text");
            entity.Property(e => e.SubmissionUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);

            // Global query filter for multi-tenancy
            // The filter is evaluated at query time, ensuring tenant isolation
            entity.HasQueryFilter(e => e.FacultyId == CurrentFacultyId);

            // Relationships
            entity.HasOne(e => e.Student)
                .WithMany(s => s.StudentAssignments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Assignment)
                .WithMany(a => a.StudentAssignments)
                .HasForeignKey(e => e.AssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.FacultyId);
            entity.HasIndex(e => new { e.StudentId, e.AssignmentId });
        });
    }

    private void ConfigureExam(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exam>(entity =>
        {
            entity.ToTable("Exam");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FacultyId).IsRequired();
            entity.Property(e => e.CourseId).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.ExamDate);
            entity.Property(e => e.RegDeadline);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);

            // Global query filter for multi-tenancy
            // The filter is evaluated at query time, ensuring tenant isolation
            entity.HasQueryFilter(e => e.FacultyId == CurrentFacultyId);

            // Relationships
            entity.HasOne(e => e.Course)
                .WithMany(c => c.Exams)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.FacultyId);
            entity.HasIndex(e => e.CourseId);
        });
    }

    private void ConfigureExamRegistration(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExamRegistration>(entity =>
        {
            entity.ToTable("ExamRegistration");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FacultyId).IsRequired();
            entity.Property(e => e.StudentId).IsRequired();
            entity.Property(e => e.ExamId).IsRequired();
            entity.Property(e => e.RegistrationDate).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);

            // Global query filter for multi-tenancy
            // The filter is evaluated at query time, ensuring tenant isolation
            entity.HasQueryFilter(e => e.FacultyId == CurrentFacultyId);

            // Relationships
            entity.HasOne(e => e.Student)
                .WithMany(s => s.ExamRegistrations)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Exam)
                .WithMany(ex => ex.ExamRegistrations)
                .HasForeignKey(e => e.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.FacultyId);
            entity.HasIndex(e => new { e.StudentId, e.ExamId });
        });
    }

    private void ConfigureStudentExamGrade(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentExamGrade>(entity =>
        {
            entity.ToTable("StudentExamGrade");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FacultyId).IsRequired();
            entity.Property(e => e.StudentId).IsRequired();
            entity.Property(e => e.ExamId).IsRequired();
            entity.Property(e => e.Passed);
            entity.Property(e => e.Points);
            entity.Property(e => e.URL).HasMaxLength(500);
            entity.Property(e => e.DateRecorded);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);

            // Global query filter for multi-tenancy
            // The filter is evaluated at query time, ensuring tenant isolation
            entity.HasQueryFilter(e => e.FacultyId == CurrentFacultyId);

            // Relationships
            entity.HasOne(e => e.Student)
                .WithMany(s => s.StudentExamGrades)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Exam)
                .WithMany(ex => ex.StudentExamGrades)
                .HasForeignKey(e => e.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.FacultyId);
            entity.HasIndex(e => new { e.StudentId, e.ExamId });
        });
    }

    private void ConfigureAttendance(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.ToTable("Attendance");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FacultyId).IsRequired();
            entity.Property(e => e.StudentId).IsRequired();
            entity.Property(e => e.CourseId).IsRequired();
            entity.Property(e => e.LectureDate).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);

            // Global query filter for multi-tenancy
            // The filter is evaluated at query time, ensuring tenant isolation
            entity.HasQueryFilter(e => e.FacultyId == CurrentFacultyId);

            // Relationships
            entity.HasOne(e => e.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Attendances)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.FacultyId);
            entity.HasIndex(e => new { e.StudentId, e.CourseId, e.LectureDate });
        });
    }
}
