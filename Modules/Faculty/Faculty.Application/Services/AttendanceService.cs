using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;

namespace Faculty.Application.Services;

/// <summary>
/// Service implementation for attendance management operations.
/// </summary>
public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepository;
    public AttendanceService(
        IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository ?? throw new ArgumentNullException(nameof(attendanceRepository));
    }

    /// <summary>
    /// Verifies that the current user is assigned to the specified course.
    /// </summary>
    private async Task VerifyTeacherAssignmentAsync(Guid courseId, string userId)
    {
        var isAssigned = await _attendanceRepository.IsTeacherAssignedToCourseAsync(userId, courseId);
        if (!isAssigned)
        {
            throw new UnauthorizedAccessException("You are not assigned to this course.");
        }
    }

    public async Task<List<StudentAttendanceDTO>> GetStudentsWithAttendanceAsync(Guid courseId, DateTime date, string userId)
    {
        // Verify teacher is assigned to course
        await VerifyTeacherAssignmentAsync(courseId, userId);

        // Get all enrolled students
        var enrollments = await _attendanceRepository.GetEnrolledStudentsAsync(courseId);

        // Get existing attendance records for the date
        var attendanceRecords = new Dictionary<int, Attendance>();
        foreach (var enrollment in enrollments)
        {
            var attendance = await _attendanceRepository.GetAttendanceAsync(enrollment.StudentId, courseId, date);
            if (attendance != null)
            {
                attendanceRecords[enrollment.StudentId] = attendance;
            }
        }

        // Build result DTOs
        var result = enrollments.Select(enrollment =>
        {
            var attendance = attendanceRecords.TryGetValue(enrollment.StudentId, out var att) ? att : null;
            return new StudentAttendanceDTO
            {
                StudentId = enrollment.Student.Id,
                IndexNumber = enrollment.Student.IndexNumber,
                FirstName = enrollment.Student.FirstName ?? string.Empty,
                LastName = enrollment.Student.LastName ?? string.Empty,
                Status = attendance?.Status,
                Note = attendance?.Note
            };
        }).ToList();

        return result;
    }

    public async Task SaveAttendanceAsync(SaveAttendanceRequestDTO request, string userId)
    {
        // Verify teacher is assigned to course
        await VerifyTeacherAssignmentAsync(request.CourseId, userId);

        // Get enrolled students to validate
        var enrollments = await _attendanceRepository.GetEnrolledStudentsAsync(request.CourseId);
        var enrolledStudentIds = enrollments.Select(e => e.StudentId).ToHashSet();

        // Validate all student IDs are enrolled
        var invalidStudentIds = request.Records
            .Select(r => r.StudentId)
            .Where(id => !enrolledStudentIds.Contains(id))
            .ToList();

        if (invalidStudentIds.Any())
        {
            throw new ArgumentException($"Invalid student IDs: {string.Join(", ", invalidStudentIds)}. These students are not enrolled in the course.");
        }

        // Create or update attendance records
        var dateOnly = DateTime.SpecifyKind(request.Date.Date, DateTimeKind.Utc);
        var facultyId = await _attendanceRepository.GetCourseFacultyIdAsync(request.CourseId);
        foreach (var record in request.Records)
        {
            var attendance = new Attendance
            {
                FacultyId = facultyId,
                StudentId = record.StudentId,
                CourseId = request.CourseId,
                LectureDate = dateOnly,
                Status = record.Status,
                Note = record.Note
            };

            await _attendanceRepository.CreateOrUpdateAttendanceAsync(attendance);
        }
    }

    public async Task<AttendanceStatisticsDTO> GetAttendanceStatisticsAsync(Guid courseId, DateTime startDate, DateTime endDate, string userId)
    {
        // Verify teacher is assigned to course
        await VerifyTeacherAssignmentAsync(courseId, userId);

        // Get all attendance records in the date range
        var attendances = await _attendanceRepository.GetAttendanceForDateRangeAsync(courseId, startDate, endDate);

        var totalRecords = attendances.Count;
        var presentCount = attendances.Count(a => a.Status == "Present");
        var absentCount = attendances.Count(a => a.Status == "Absent");
        var lateCount = attendances.Count(a => a.Status == "Late");

        var presentPercentage = totalRecords > 0 ? (double)presentCount / totalRecords * 100 : 0;
        var absentPercentage = totalRecords > 0 ? (double)absentCount / totalRecords * 100 : 0;
        var latePercentage = totalRecords > 0 ? (double)lateCount / totalRecords * 100 : 0;

        return new AttendanceStatisticsDTO
        {
            TotalRecords = totalRecords,
            PresentCount = presentCount,
            AbsentCount = absentCount,
            LateCount = lateCount,
            PresentPercentage = Math.Round(presentPercentage, 2),
            AbsentPercentage = Math.Round(absentPercentage, 2),
            LatePercentage = Math.Round(latePercentage, 2)
        };
    }

    public async Task<byte[]> ExportAttendanceReportAsync(Guid courseId, DateTime date, string userId)
    {
        // Verify teacher is assigned to course
        await VerifyTeacherAssignmentAsync(courseId, userId);

        // Get students with attendance
        var students = await GetStudentsWithAttendanceAsync(courseId, date, userId);

        // Generate CSV report (can be extended to Excel/PDF later)
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Index Number,First Name,Last Name,Status,Note");

        foreach (var student in students)
        {
            csv.AppendLine($"{student.IndexNumber},{student.FirstName},{student.LastName},{student.Status ?? "Not Recorded"},{student.Note ?? ""}");
        }

        return System.Text.Encoding.UTF8.GetBytes(csv.ToString());
    }
}

