using Faculty.Core.Interfaces;

namespace Faculty.Infrastructure.Schemas;

/// <summary>
/// Persistence schema for Enrollment entity.
/// </summary>
public class EnrollmentSchema : ITenantAware
{
    public int Id { get; set; }
    public Guid FacultyId { get; set; }
    public int StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string? Status { get; set; }
    public int? Grade { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public StudentSchema Student { get; set; } = null!;
    public CourseSchema Course { get; set; } = null!;
}

