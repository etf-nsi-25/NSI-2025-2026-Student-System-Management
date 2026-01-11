using Faculty.Core.Interfaces;

namespace Faculty.Infrastructure.Schemas;

/// <summary>
/// Persistence schema for CourseAssignment entity.
/// </summary>
public class CourseAssignmentSchema : ITenantAware
{
    public int Id { get; set; }
    public Guid FacultyId { get; set; }
    public int TeacherId { get; set; }
    public Guid CourseId { get; set; }
    public string? Role { get; set; }
    public int AcademicYearId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public TeacherSchema Teacher { get; set; } = null!;
    public CourseSchema Course { get; set; } = null!;
}

