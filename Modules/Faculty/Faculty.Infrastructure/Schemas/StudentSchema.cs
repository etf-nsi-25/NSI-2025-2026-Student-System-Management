using Faculty.Core.Interfaces;

namespace Faculty.Infrastructure.Schemas;

/// <summary>
/// Persistence schema for Student entity.
/// </summary>
public class StudentSchema : ITenantAware
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
    public ICollection<EnrollmentSchema> Enrollments { get; set; } = new List<EnrollmentSchema>();
    public ICollection<StudentAssignmentSchema> StudentAssignments { get; set; } = new List<StudentAssignmentSchema>();
    public ICollection<ExamRegistrationSchema> ExamRegistrations { get; set; } = new List<ExamRegistrationSchema>();
    public ICollection<StudentExamGradeSchema> StudentExamGrades { get; set; } = new List<StudentExamGradeSchema>();
    public ICollection<AttendanceSchema> Attendances { get; set; } = new List<AttendanceSchema>();
}

