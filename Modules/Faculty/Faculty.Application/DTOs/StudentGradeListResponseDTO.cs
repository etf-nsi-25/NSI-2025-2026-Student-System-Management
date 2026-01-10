namespace Faculty.Application.DTOs;

public class StudentGradeListResponseDTO
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = null!;
    public IEnumerable<GradeResponseDTO> Grades { get; set; } = Enumerable.Empty<GradeResponseDTO>();
}