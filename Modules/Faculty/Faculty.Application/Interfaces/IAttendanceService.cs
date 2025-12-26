using Faculty.Application.DTOs;

namespace Faculty.Application.Interfaces;

/// <summary>
/// Service interface for attendance management operations.
/// </summary>
public interface IAttendanceService
{
    Task<List<StudentAttendanceDTO>> GetStudentsWithAttendanceAsync(Guid courseId, DateTime date, string userId);
    Task SaveAttendanceAsync(SaveAttendanceRequestDTO request, string userId);
    Task<AttendanceStatisticsDTO> GetAttendanceStatisticsAsync(Guid courseId, DateTime startDate, DateTime endDate, string userId);
    Task<byte[]> ExportAttendanceReportAsync(Guid courseId, DateTime date, string userId);
}

