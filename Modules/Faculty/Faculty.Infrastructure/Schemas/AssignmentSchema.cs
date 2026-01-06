using Faculty.Core.Interfaces;

namespace Faculty.Infrastructure.Schemas;

/// <summary>
/// Persistence schema for Assignment entity.
/// </summary>
public class AssignmentSchema : ITenantAware
{
    public int Id { get; set; }
    public Guid FacultyId { get; set; }
    public Guid CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int? MaxPoints { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public CourseSchema Course { get; set; } = null!;
    public ICollection<StudentAssignmentSchema> StudentAssignments { get; set; } = new List<StudentAssignmentSchema>();
}

