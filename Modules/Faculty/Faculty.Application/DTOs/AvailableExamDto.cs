namespace Faculty.Application.DTOs;

public class AvailableExamDto
{
    public int ExamId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public DateTime? RegistrationDeadline { get; set; }
}
