namespace Faculty.Application.DTOs;

public class GradeUpdateRequestDTO
{
    public double? Points { get; set; }
    public bool? Passed { get; set; }
    public DateTime? DateRecorded { get; set; }
}