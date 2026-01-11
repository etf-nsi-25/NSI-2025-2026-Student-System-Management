using Faculty.Core.Entities;
using Faculty.Core.Interfaces;

namespace Faculty.Infrastructure.Schemas;

/// <summary>
/// Persistence schema for Course entity.
/// </summary>
public class CourseSchema : ITenantAware
{
    public Guid Id { get; set; }
    public Guid FacultyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public CourseType Type { get; set; }
    public string? ProgramId { get; set; }
    public int ECTS { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<CourseAssignmentSchema> CourseAssignments { get; set; } = new List<CourseAssignmentSchema>();
    public ICollection<EnrollmentSchema> Enrollments { get; set; } = new List<EnrollmentSchema>();
    public ICollection<AssignmentSchema> Assignments { get; set; } = new List<AssignmentSchema>();
    public ICollection<ExamSchema> Exams { get; set; } = new List<ExamSchema>();
    public ICollection<AttendanceSchema> Attendances { get; set; } = new List<AttendanceSchema>();
}

