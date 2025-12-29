namespace Faculty.Application.DTOs;

public class ExamRegistrationResponseDto
{
    public int RegistrationId { get; set; }
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public DateTime? ExamDate { get; set; }
    public string? ExamName { get; set; }
}
