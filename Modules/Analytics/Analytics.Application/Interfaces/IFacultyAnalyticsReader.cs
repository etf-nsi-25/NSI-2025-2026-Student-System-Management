using Analytics.Application.DTOs;

namespace Analytics.Application.Interfaces;

public interface IFacultyAnalyticsReader
{
	Task<List<FacultyEnrollmentRow>> GetEnrollmentsAsync(
		string userId,
		Guid facultyId,
		CancellationToken ct);
}
