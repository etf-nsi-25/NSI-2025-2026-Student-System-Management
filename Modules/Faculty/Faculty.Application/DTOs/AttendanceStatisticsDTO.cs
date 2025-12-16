namespace Faculty.Application.DTOs;

/// <summary>
/// DTO representing attendance statistics for a course and time period.
/// </summary>
public class AttendanceStatisticsDTO
{
    public int TotalRecords { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public double PresentPercentage { get; set; }
    public double AbsentPercentage { get; set; }
    public double LatePercentage { get; set; }
}

