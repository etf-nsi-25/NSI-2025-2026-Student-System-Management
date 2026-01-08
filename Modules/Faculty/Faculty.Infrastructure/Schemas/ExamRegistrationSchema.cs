using Faculty.Core.Interfaces;

namespace Faculty.Infrastructure.Schemas;

/// <summary>
/// Persistence schema for ExamRegistration entity.
/// </summary>
public class ExamRegistrationSchema : ITenantAware
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
    public StudentSchema Student { get; set; } = null!;
    public ExamSchema Exam { get; set; } = null!;
}

