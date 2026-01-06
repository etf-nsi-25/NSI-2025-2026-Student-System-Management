namespace Faculty.Application.DTOs;

public class GradeRequestDTO
{
    public int StudentId { get; set; }
    public int ExamId { get; set; }
    public float Points { get; set; }
    public bool Passed { get; set; }         
    public string? Url { get; set; }
}