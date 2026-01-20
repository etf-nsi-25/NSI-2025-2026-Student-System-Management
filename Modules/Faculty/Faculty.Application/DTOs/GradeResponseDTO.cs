namespace Faculty.Application.DTOs;

public class GradeResponseDTO
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public double? Points { get; set; }
    public bool? Passed { get; set; }
    public DateTime? DateRecorded { get; set; }
    public string? Url { get; set; }
}