using Faculty.Core.Interfaces;

namespace Faculty.Infrastructure.Schemas;

/// <summary>
/// Persistence schema for Exam entity.
/// </summary>
public class ExamSchema : ITenantAware
{
    public int Id { get; set; }
    public Guid FacultyId { get; set; }
    public Guid CourseId { get; set; }
    public string? Name { get; set; }
    public DateTime? ExamDate { get; set; }
    public DateTime? RegDeadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public CourseSchema Course { get; set; } = null!;
    public ICollection<ExamRegistrationSchema> ExamRegistrations { get; set; } = new List<ExamRegistrationSchema>();
    public ICollection<StudentExamGradeSchema> StudentExamGrades { get; set; } = new List<StudentExamGradeSchema>();
}

