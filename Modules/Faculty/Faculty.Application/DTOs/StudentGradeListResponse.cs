namespace Faculty.Application.DTOs;

public class StudentGradeListResponse
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = null!;
    public IEnumerable<GradeResponse> Grades { get; set; } = Enumerable.Empty<GradeResponse>();
}