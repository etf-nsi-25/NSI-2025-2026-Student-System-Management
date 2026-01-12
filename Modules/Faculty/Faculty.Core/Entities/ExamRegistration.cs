using Faculty.Core.Interfaces;

namespace Faculty.Core.Entities;

/// <summary>
/// Represents a student's registration for an exam.
/// </summary>
public class ExamRegistration : ITenantAware
{
    public int Id { get; set; }
    public Guid FacultyId { get; set; }
    public int StudentId { get; set; }
    public int ExamId { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Exam Exam { get; set; } = null!;
}

