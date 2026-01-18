namespace Faculty.Application.DTOs;

/// <summary>
/// DTO representing monthly calendar events for a student.
/// </summary>
public class MonthlyCalendarDTO
{
    public string CurrentMonth { get; set; } = string.Empty; // ISO date string
    public List<HighlightedDayDTO> HighlightedDays { get; set; } = new();
}

/// <summary>
/// DTO representing a highlighted day in the calendar.
/// </summary>
public class HighlightedDayDTO
{
    public int Day { get; set; }
    public string EventType { get; set; } = "Exam"; // Exam, Assignment, Midterm, Quiz, PublicHoliday
    public string? EventName { get; set; } // Optional: Name of the exam/assignment (e.g., "Final Exam", "Homework 1")
    public string? CourseCode { get; set; } // Optional: Course code (e.g., "RSRV", "MPVI")
}

