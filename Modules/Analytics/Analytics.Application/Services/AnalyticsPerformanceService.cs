using Analytics.Application.DTOs;
using Analytics.Application.Interfaces;

namespace Analytics.Application.Services;

public sealed class AcademicPerformanceService : IAcademicPerformanceService
{
	private readonly IFacultyAnalyticsReader _reader;

	public AcademicPerformanceService(IFacultyAnalyticsReader reader)
	{
		_reader = reader;
	}

	public async Task<AcademicPerformanceAnalyticsDto> GetAsync(
		string userId,
		Guid facultyId,
		CancellationToken ct)
	{
		var enrollments = await _reader.GetEnrollmentsAsync(userId, facultyId, ct);

		var totalCourses = enrollments.Count;
		var passed = enrollments.Where(x => x.Grade is >= 6).ToList();

		var requiredEcts = enrollments.Sum(x => x.Ects);
		var totalEcts = passed.Sum(x => x.Ects);

		decimal gpa = 0m;
		if (totalEcts > 0)
		{
			var weighted = passed.Sum(x => (decimal)(x.Grade!.Value * x.Ects));
			gpa = weighted / totalEcts;
		}

		decimal completion = totalCourses > 0
			? (decimal)passed.Count / totalCourses * 100m
			: 0m;

		return new AcademicPerformanceAnalyticsDto
		{
			Gpa = Math.Round(gpa, 2),
			TotalECTS = totalEcts,
			RequiredECTS = requiredEcts,
			SemesterCompletion = Math.Round(completion, 2),
			GradeDistribution = enrollments
				.Where(x => x.Grade.HasValue)
				.Select(x => new CourseGradeDto
				{
					CourseName = x.CourseName,
					Grade = x.Grade!.Value
				})
				.ToList()
		};
	}
}
