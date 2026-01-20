using Faculty.Application.DTOs;

namespace Faculty.Application.Interfaces;

public interface IStudentAnalyticsService
{
    Task<StudentSummaryDTO> GetStudentSummaryAsync(string userId, CancellationToken cancellationToken = default);

    Task<WeeklyScheduleDTO> GetWeeklyScheduleAsync(string userId, DateTime startDate, CancellationToken cancellationToken = default);

    Task<MonthlyCalendarDTO> GetMonthlyCalendarAsync(string userId, int year, int month, CancellationToken cancellationToken = default);

    Task<StudentAttendanceStatsDTO> GetAttendanceStatsAsync(string userId, Guid courseId, CancellationToken cancellationToken = default);

    Task<SubjectProgressDTO> GetSubjectProgressAsync(string userId, Guid? semesterId, CancellationToken cancellationToken = default);
}

