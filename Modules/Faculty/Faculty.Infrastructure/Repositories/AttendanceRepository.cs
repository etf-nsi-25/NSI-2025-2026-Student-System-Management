using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for attendance operations using Entity Framework Core.
/// </summary>
public class AttendanceRepository : IAttendanceRepository
{
    private readonly FacultyDbContext _context;

    public AttendanceRepository(FacultyDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<Enrollment>> GetEnrolledStudentsAsync(Guid courseId)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Where(e => e.CourseId == courseId)
            .OrderBy(e => e.Student.LastName)
            .ThenBy(e => e.Student.FirstName)
            .ToListAsync();
    }

    public async Task<Attendance?> GetAttendanceAsync(int studentId, Guid courseId, DateTime date)
    {
        // Normalize date to compare only date part (ignore time)
        var dateOnly = date.Date;
        
        return await _context.Attendances
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => 
                a.StudentId == studentId && 
                a.CourseId == courseId && 
                a.LectureDate.Date == dateOnly);
    }

    public async Task<List<Attendance>> GetAttendanceForDateRangeAsync(Guid courseId, DateTime startDate, DateTime endDate)
    {
        var startDateOnly = startDate.Date;
        var endDateOnly = endDate.Date;
        
        return await _context.Attendances
            .Include(a => a.Student)
            .Where(a => 
                a.CourseId == courseId && 
                a.LectureDate.Date >= startDateOnly && 
                a.LectureDate.Date <= endDateOnly)
            .ToListAsync();
    }

    public async Task<Attendance> CreateOrUpdateAttendanceAsync(Attendance attendance)
    {
        // Check if attendance record already exists
        var existing = await GetAttendanceAsync(attendance.StudentId, attendance.CourseId, attendance.LectureDate);
        
        if (existing != null)
        {
            // Update existing record
            existing.Status = attendance.Status;
            existing.Note = attendance.Note;
            existing.UpdatedAt = DateTime.UtcNow;
            _context.Attendances.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }
        else
        {
            // Create new record
            attendance.CreatedAt = DateTime.UtcNow;
            attendance.LectureDate = attendance.LectureDate.Date; // Ensure only date part
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }
    }

    public async Task<bool> IsTeacherAssignedToCourseAsync(string userId, Guid courseId)
    {
        // First get the teacher by userId
        var teacher = await GetTeacherByUserIdAsync(userId);
        if (teacher == null)
            return false;

        // Check if teacher is assigned to the course
        return await _context.CourseAssignments
            .AnyAsync(ca => ca.TeacherId == teacher.Id && ca.CourseId == courseId);
    }

    public async Task<Teacher?> GetTeacherByUserIdAsync(string userId)
    {
        return await _context.Teachers
            .FirstOrDefaultAsync(t => t.UserId == userId);
    }

    public async Task<Guid> GetCourseFacultyIdAsync(Guid courseId)
    {
        var facultyId = await _context.Courses
            .Where(c => c.Id == courseId)
            .Select(c => c.FacultyId)
            .FirstOrDefaultAsync();

        if (facultyId == Guid.Empty)
        {
            throw new ArgumentException($"Course with ID '{courseId}' was not found.");
        }

        return facultyId;
    }
}

