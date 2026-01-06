using Faculty.Core.Interfaces;

namespace Faculty.Infrastructure.Schemas;

/// <summary>
/// Persistence schema for Teacher entity.
/// </summary>
public class TeacherSchema : ITenantAware
{
    public int Id { get; set; }
    public Guid FacultyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Office { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<CourseAssignmentSchema> CourseAssignments { get; set; } = new List<CourseAssignmentSchema>();
}

