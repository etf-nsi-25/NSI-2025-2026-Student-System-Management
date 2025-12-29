using Analytics.Application.DTOs;
using Analytics.Application.Interfaces;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Readers;

public sealed class FacultyAnalyticsReader : IFacultyAnalyticsReader
{
	private readonly FacultyDbContext _facultyDb;

	public FacultyAnalyticsReader(FacultyDbContext facultyDb)
	{
		_facultyDb = facultyDb;
	}

	public async Task<List<FacultyEnrollmentRow>> GetEnrollmentsAsync(
		string userId,
		Guid facultyId,
		CancellationToken ct)
	{
		return await _facultyDb.Enrollments
			.AsNoTracking()
			.Where(e =>
				e.FacultyId == facultyId &&
				e.Student.UserId == userId)
			.Select(e => new FacultyEnrollmentRow
			{
				CourseName = e.Course.Name,
				Ects = e.Course.ECTS,
				Grade = e.Grade
			})
			.ToListAsync(ct);
	}
}
