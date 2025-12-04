using Faculty.Core.Interfaces;

namespace Faculty.Core.Entities;

/// <summary>
/// Represents an assignment for a course.
/// </summary>
public class Assignment : ITenantAware
{
    public int Id { get; set; }
    public Guid FacultyId { get; set; }
    public int CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int? MaxPoints { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Course Course { get; set; } = null!;
    public ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();
}

