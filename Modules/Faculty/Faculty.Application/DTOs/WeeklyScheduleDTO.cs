namespace Faculty.Application.DTOs;

public class WeeklyScheduleDTO
{
    public int StartHour { get; set; }
    public int EndHour { get; set; }
    public List<CourseBlockDTO> Blocks { get; set; } = new();
}

public class CourseBlockDTO
{
    public string Id { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Day { get; set; } = string.Empty;
    public int StartMinutes { get; set; }
    public int EndMinutes { get; set; }
    public string? Type { get; set; }
}

