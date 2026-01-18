namespace Faculty.Application.DTOs;

/// <summary>
/// DTO representing attendance statistics for a student in a specific course.
/// </summary>
public class StudentAttendanceStatsDTO
{
    public string ContextLabel { get; set; } = string.Empty; // e.g., "for RSRV"
    public List<AttendanceItemDTO> Items { get; set; } = new();
}

/// <summary>
/// DTO representing an attendance item (Lectures, Tutorials, etc.).
/// </summary>
public class AttendanceItemDTO
{
    public string Label { get; set; } = string.Empty; // "Lectures" or "Tutorials"
    public double Percent { get; set; }
    public int PresentCount { get; set; } // Number of present/late attendances
    public int TotalCount { get; set; } // Total number of attendances for this type
}

