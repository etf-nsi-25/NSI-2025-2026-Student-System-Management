namespace Analytics.Application.DTOs;

public sealed class AcademicPerformanceAnalyticsDto
{
	public decimal Gpa { get; init; }
	public int TotalECTS { get; init; }
	public int RequiredECTS { get; init; }
	public decimal SemesterCompletion { get; init; }
	public List<CourseGradeDto> GradeDistribution { get; init; } = new();
}

public sealed class CourseGradeDto
{
	public Guid CourseId { get; init; }
	public string CourseName { get; init; } = string.Empty;
	public int Grade { get; init; }
}
