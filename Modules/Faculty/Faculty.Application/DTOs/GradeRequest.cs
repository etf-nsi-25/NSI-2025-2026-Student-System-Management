namespace Faculty.Application.DTOs;

public class GradeRequest
{
    public int StudentId { get; set; }
    public int ExamId { get; set; }
    public float Points { get; set; }
    public bool Passed { get; set; }         
    public string? Url { get; set; }
}