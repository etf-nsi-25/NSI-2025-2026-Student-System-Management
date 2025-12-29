namespace Analytics.Application.DTOs;

public sealed class FacultyEnrollmentRow
{
	public string CourseName { get; init; } = string.Empty;
	public int Ects { get; init; }
	public int? Grade { get; init; }
}
