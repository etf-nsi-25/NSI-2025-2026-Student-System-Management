using Faculty.Core.Interfaces;

namespace Faculty.Core.Entities;

/// <summary>
/// Represents the assignment of a teacher to a course with a specific role.
/// </summary>
public class CourseAssignment : ITenantAware
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
    public Teacher Teacher { get; set; } = null!;
    public Course Course { get; set; } = null!;
}

