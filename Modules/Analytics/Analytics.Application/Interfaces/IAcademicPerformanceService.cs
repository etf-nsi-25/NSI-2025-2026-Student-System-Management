using Analytics.Application.DTOs;

namespace Analytics.Application.Interfaces;

public interface IAcademicPerformanceService
{
	Task<AcademicPerformanceAnalyticsDto> GetAsync(string userId, Guid facultyId, CancellationToken ct);
}
