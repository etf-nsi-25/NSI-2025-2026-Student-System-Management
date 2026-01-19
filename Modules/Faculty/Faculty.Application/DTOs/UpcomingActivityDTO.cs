namespace Faculty.Application.DTOs;

/// <summary>
/// DTO representing an upcoming activity for a professor's course.
/// </summary>
public class UpcomingActivityDTO
{
    public DateTime Date { get; set; }

    public TimeSpan? Time { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string? Location { get; set; }

    public Guid CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;
}
