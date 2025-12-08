using Faculty.Core.Interfaces;

namespace Faculty.Core.Entities;

/// <summary>
/// Represents an exam for a course.
/// </summary>
public class Exam : ITenantAware
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
    public Course Course { get; set; } = null!;
    public ICollection<ExamRegistration> ExamRegistrations { get; set; } = new List<ExamRegistration>();
    public ICollection<StudentExamGrade> StudentExamGrades { get; set; } = new List<StudentExamGrade>();
}

