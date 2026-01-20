namespace Faculty.Application.DTOs;

public class MonthlyCalendarDTO
{
    public string CurrentMonth { get; set; } = string.Empty;
    public List<HighlightedDayDTO> HighlightedDays { get; set; } = new();
}

public class HighlightedDayDTO
{
    public int Day { get; set; }
    public string EventType { get; set; } = "Exam";
    public string? EventName { get; set; }
    public string? CourseCode { get; set; }
}

