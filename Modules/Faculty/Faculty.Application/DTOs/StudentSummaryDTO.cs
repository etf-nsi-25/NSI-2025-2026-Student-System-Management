namespace Faculty.Application.DTOs;

/// <summary>
/// DTO representing student summary with GPA and passed/total subjects.
/// </summary>
public class StudentSummaryDTO
{
    public double gpa { get; set; }
    public int PassedSubjects { get; set; }
    public int TotalSubjects { get; set; }
}

