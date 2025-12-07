using Faculty.Core.Interfaces;

namespace Faculty.Core.Entities;

/// <summary>
/// Represents a student's submission for an assignment.
/// </summary>
public class StudentAssignment : ITenantAware
{
    public int Id { get; set; }
    public Guid FacultyId { get; set; }
    public int StudentId { get; set; }
    public int AssignmentId { get; set; }
    public DateTime? SubmissionDate { get; set; }
    public int? Points { get; set; }
    public int? Grade { get; set; }
    public string? Feedback { get; set; }
    public string? SubmissionUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Assignment Assignment { get; set; } = null!;
}

