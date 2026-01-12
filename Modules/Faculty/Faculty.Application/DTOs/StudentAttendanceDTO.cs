namespace Faculty.Application.DTOs;

/// <summary>
/// DTO representing a student with their attendance status for a specific date.
/// </summary>
public class StudentAttendanceDTO
{
    public int StudentId { get; set; }
    public string IndexNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Status { get; set; } // Present, Absent, Late
    public string? Note { get; set; }
}

