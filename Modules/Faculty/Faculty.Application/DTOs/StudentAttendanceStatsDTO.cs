namespace Faculty.Application.DTOs;

public class StudentAttendanceStatsDTO
{
    public string ContextLabel { get; set; } = string.Empty;
    public List<AttendanceItemDTO> Items { get; set; } = new();
}

public class AttendanceItemDTO
{
    public string Label { get; set; } = string.Empty;
    public double Percent { get; set; }
    public int PresentCount { get; set; }
    public int TotalCount { get; set; }
}

