using Faculty.Core.Interfaces;

namespace Faculty.Core.Entities;

/// <summary>
/// Represents a student's grade for an exam.
/// </summary>
public class StudentExamGrade : ITenantAware
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
    public Student Student { get; set; } = null!;
    public Exam Exam { get; set; } = null!;
}

