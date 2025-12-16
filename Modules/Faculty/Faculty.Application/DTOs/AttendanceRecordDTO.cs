using System.ComponentModel.DataAnnotations;

namespace Faculty.Application.DTOs;

/// <summary>
/// DTO for creating or updating an attendance record.
/// </summary>
public class AttendanceRecordDTO
{
    [Required]
    public int StudentId { get; set; }
    
    [Required]
    [RegularExpression("^(Present|Absent|Late)$", ErrorMessage = "Status must be Present, Absent, or Late.")]
    public string Status { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Note { get; set; }
}

/// <summary>
/// DTO for bulk saving attendance records.
/// </summary>
public class SaveAttendanceRequestDTO
{
    [Required]
    public Guid CourseId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "At least one attendance record is required.")]
    public List<AttendanceRecordDTO> Records { get; set; } = new();
}

