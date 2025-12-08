using Faculty.Core.Interfaces;

namespace Faculty.Core.Entities;

/// <summary>
/// Represents a student in the faculty system.
/// </summary>
public class Student : ITenantAware
{
    public int Id { get; set; }
    public Guid FacultyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string IndexNumber { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? EnrollmentDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();
    public ICollection<ExamRegistration> ExamRegistrations { get; set; } = new List<ExamRegistration>();
    public ICollection<StudentExamGrade> StudentExamGrades { get; set; } = new List<StudentExamGrade>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}

