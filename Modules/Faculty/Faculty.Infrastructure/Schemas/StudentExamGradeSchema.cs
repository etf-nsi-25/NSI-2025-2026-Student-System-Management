using Faculty.Core.Interfaces;

namespace Faculty.Infrastructure.Schemas;

/// <summary>
/// Persistence schema for StudentExamGrade entity.
/// </summary>
public class StudentExamGradeSchema : ITenantAware
{
    public int Id { get; set; }
    public Guid FacultyId { get; set; }
    public int StudentId { get; set; }
    public int ExamId { get; set; }
    public bool? Passed { get; set; }
    public double? Points { get; set; }
    public string? URL { get; set; }
    public DateTime? DateRecorded { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public StudentSchema Student { get; set; } = null!;
    public ExamSchema Exam { get; set; } = null!;
}

