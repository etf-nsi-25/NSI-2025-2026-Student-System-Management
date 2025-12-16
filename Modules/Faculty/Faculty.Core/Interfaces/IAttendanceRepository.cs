using Faculty.Core.Entities;

namespace Faculty.Core.Interfaces;

/// <summary>
/// Repository interface for attendance operations.
/// </summary>
public interface IAttendanceRepository
{
    Task<List<Enrollment>> GetEnrolledStudentsAsync(Guid courseId);
    Task<Attendance?> GetAttendanceAsync(int studentId, Guid courseId, DateTime date);
    Task<List<Attendance>> GetAttendanceForDateRangeAsync(Guid courseId, DateTime startDate, DateTime endDate);
    Task<Attendance> CreateOrUpdateAttendanceAsync(Attendance attendance);
    Task<bool> IsTeacherAssignedToCourseAsync(string userId, Guid courseId);
    Task<Teacher?> GetTeacherByUserIdAsync(string userId);

    Task<Guid> GetCourseFacultyIdAsync(Guid courseId);
}

