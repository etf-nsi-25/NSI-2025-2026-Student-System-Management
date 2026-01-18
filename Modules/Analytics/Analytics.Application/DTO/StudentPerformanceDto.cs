namespace Analytics.Application.DTO;

public class StudentPerformanceDto
{
    public required string StudentId { get; set; }   // Index
    public required string StudentName { get; set; }
    public required string Attendance { get; set; }  // npr. "7/10"
    public int Score { get; set; }          // 0-100
    public bool Passed { get; set; }        // Score >= 55
}