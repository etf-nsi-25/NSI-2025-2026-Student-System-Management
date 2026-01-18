namespace Faculty.Application.DTOs;

/// <summary>
/// DTO representing a weekly schedule for a student.
/// </summary>
public class WeeklyScheduleDTO
{
    public int StartHour { get; set; }
    public int EndHour { get; set; }
    public List<CourseBlockDTO> Blocks { get; set; } = new();
}

/// <summary>
/// DTO representing a course block in the schedule.
/// </summary>
public class CourseBlockDTO
{
    public string Id { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty; // Course code
    public string Day { get; set; } = string.Empty; // Mon, Tue, Wed, Thu, Fri
    public int StartMinutes { get; set; } // minutes since 00:00
    public int EndMinutes { get; set; } // minutes since 00:00
    public string? Type { get; set; } // "Lecture" or "Tutorial" - from Attendance Note field
}

