namespace Faculty.Application.DTOs;

/// <summary>
/// DTO representing a course assigned to a professor with student count.
/// </summary>
public class ProfessorCourseDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int StudentCount { get; set; }
}
