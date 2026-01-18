using Faculty.Application.DTOs;

namespace Faculty.Application.Interfaces;

/// <summary>
/// Service interface for student analytics operations.
/// </summary>
public interface IStudentAnalyticsService
{
    /// <summary>
    /// Gets student summary with GPA and passed/total subjects.
    /// </summary>
    Task<StudentSummaryDTO> GetStudentSummaryAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets weekly schedule for a student starting from the specified date.
    /// </summary>
    Task<WeeklyScheduleDTO> GetWeeklyScheduleAsync(string userId, DateTime startDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets monthly calendar events for a student.
    /// </summary>
    Task<MonthlyCalendarDTO> GetMonthlyCalendarAsync(string userId, int year, int month, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets attendance statistics for a student in a specific course.
    /// </summary>
    Task<StudentAttendanceStatsDTO> GetAttendanceStatsAsync(string userId, Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subject progress for a student in a specific semester.
    /// </summary>
    Task<SubjectProgressDTO> GetSubjectProgressAsync(string userId, Guid? semesterId, CancellationToken cancellationToken = default);
}

