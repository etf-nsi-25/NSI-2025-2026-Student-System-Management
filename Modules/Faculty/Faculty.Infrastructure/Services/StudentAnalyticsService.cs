using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Schemas;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Services;

/// <summary>
/// Service implementation for student analytics operations.
/// </summary>
public class StudentAnalyticsService : IStudentAnalyticsService
{
    private readonly IStudentExamRegistrationRepository _studentExamRepository;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly FacultyDbContext _context;

    public StudentAnalyticsService(
        IStudentExamRegistrationRepository studentExamRepository,
        IAttendanceRepository attendanceRepository,
        FacultyDbContext context)
    {
        _studentExamRepository = studentExamRepository ?? throw new ArgumentNullException(nameof(studentExamRepository));
        _attendanceRepository = attendanceRepository ?? throw new ArgumentNullException(nameof(attendanceRepository));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<StudentSummaryDTO> GetStudentSummaryAsync(string userId, CancellationToken cancellationToken = default)
    {
        var student = await _studentExamRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student == null)
        {
            throw new InvalidOperationException($"Student with userId '{userId}' not found.");
        }

        // Get all enrollments for the student
        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == student.Id)
            .ToListAsync(cancellationToken);

        // Passed subjects: enrollments with grade >= 6 (passing grade)
        var passedEnrollments = enrollments.Where(e => e.Grade.HasValue && e.Grade >= 6).ToList();
        
        // Total subjects: all enrollments (regardless of whether they have a grade or not)
        var totalEnrollments = enrollments.Count;

        // Calculate GPA: simple average (sum of grades / number of subjects), without ECTS weighting
        // Only count passed subjects (grade >= 6) for GPA calculation
        double gpa = 0.0;
        int subjectCount = 0;
        foreach (var enrollment in passedEnrollments)
        {
            if (enrollment.Grade.HasValue)
            {
                gpa += enrollment.Grade.Value;
                subjectCount++;
            }
        }

        if (subjectCount > 0)
        {
            gpa = gpa / subjectCount;
        }

        return new StudentSummaryDTO
        {
            gpa = Math.Round(gpa, 2),
            PassedSubjects = passedEnrollments.Count,
            TotalSubjects = totalEnrollments
        };
    }

    public async Task<WeeklyScheduleDTO> GetWeeklyScheduleAsync(string userId, DateTime startDate, CancellationToken cancellationToken = default)
    {
        var student = await _studentExamRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student == null)
        {
            throw new InvalidOperationException($"Student with userId '{userId}' not found.");
        }

        // Get start of week (Monday) - ensure UTC
        var startDateUtc = startDate.Kind == DateTimeKind.Utc 
            ? startDate 
            : DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        
        var startOfWeek = startDateUtc.Date;
        while (startOfWeek.DayOfWeek != DayOfWeek.Monday)
        {
            startOfWeek = startOfWeek.AddDays(-1);
        }
        var endOfWeek = startOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

        // Get all enrollments for student
        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == student.Id)
            .ToListAsync(cancellationToken);

        var courseIds = enrollments.Select(e => e.CourseId).ToList();

        // Get all attendances for enrolled courses in this week
        // Use actual attendance times from LectureDate instead of hardcoded slots
        var attendances = await _context.Attendances
            .Include(a => a.Course)
            .Where(a => a.StudentId == student.Id &&
                       courseIds.Contains(a.CourseId) &&
                       a.LectureDate >= startOfWeek &&
                       a.LectureDate <= endOfWeek)
            .OrderBy(a => a.LectureDate)
            .ToListAsync(cancellationToken);

        var blocks = new List<CourseBlockDTO>();

        // Process each attendance individually - no grouping to avoid losing data
        // Group by day first to calculate durations and resolve overlaps
        var attendancesByDay = attendances
            .GroupBy(a => a.LectureDate.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.OrderBy(a => a.LectureDate).ToList());

        foreach (var dayGroup in attendancesByDay)
        {
            var dayOfWeek = dayGroup.Key;
            var dayName = dayOfWeek switch
            {
                DayOfWeek.Monday => "Mon",
                DayOfWeek.Tuesday => "Tue",
                DayOfWeek.Wednesday => "Wed",
                DayOfWeek.Thursday => "Thu",
                DayOfWeek.Friday => "Fri",
                _ => null
            };

            if (dayName == null) continue; // Skip weekends

            var dayAttendances = dayGroup.Value.OrderBy(a => a.LectureDate).ToList();
            var dayBlocks = new List<(CourseBlockDTO Block, int StartMinutes, int EndMinutes)>();

            // Process each attendance for this day
            for (int i = 0; i < dayAttendances.Count; i++)
            {
                var attendance = dayAttendances[i];
                var course = await _context.Courses.FindAsync([attendance.CourseId], cancellationToken);
                if (course == null) continue;

                var attendanceDate = attendance.LectureDate;
                var startMinutes = attendanceDate.Hour * 60 + attendanceDate.Minute;

                // Calculate duration based on next attendance for same course on same day
                // If no next attendance for same course, use next attendance for any course as reference
                // If no next attendance at all, use a reasonable default (90 minutes)
                int endMinutes;
                
                // First, try to find next attendance for the same course on the same day
                var nextSameCourse = dayAttendances
                    .Skip(i + 1)
                    .FirstOrDefault(a => a.CourseId == attendance.CourseId);
                
                if (nextSameCourse != null)
                {
                    // Use the time difference to the next attendance of the same course
                    var nextStartMinutes = nextSameCourse.LectureDate.Hour * 60 + nextSameCourse.LectureDate.Minute;
                    var duration = nextStartMinutes - startMinutes;
                    // Ensure minimum duration of 30 minutes and maximum of 4 hours (240 minutes)
                    duration = Math.Max(30, Math.Min(240, duration));
                    endMinutes = startMinutes + duration;
                }
                else
                {
                    // No next attendance for same course - check next attendance for any course
                    var nextAnyCourse = dayAttendances.Skip(i + 1).FirstOrDefault();
                    if (nextAnyCourse != null)
                    {
                        var nextStartMinutes = nextAnyCourse.LectureDate.Hour * 60 + nextAnyCourse.LectureDate.Minute;
                        var duration = nextStartMinutes - startMinutes;
                        // Ensure minimum duration of 30 minutes and maximum of 4 hours
                        duration = Math.Max(30, Math.Min(240, duration));
                        endMinutes = startMinutes + duration;
                    }
                    else
                    {
                        // Last attendance of the day - use default duration based on type
                        var isTutorial = !string.IsNullOrWhiteSpace(attendance.Note) && 
                                       attendance.Note.Contains("Tutorial", StringComparison.OrdinalIgnoreCase);
                        var defaultDuration = isTutorial ? 60 : 90; // Default: 60 min for tutorials, 90 min for lectures
                        endMinutes = startMinutes + defaultDuration;
                    }
                }

                // Determine type from Note field
                string? type = null;
                if (!string.IsNullOrWhiteSpace(attendance.Note))
                {
                    if (attendance.Note.Contains("Tutorial", StringComparison.OrdinalIgnoreCase))
                    {
                        type = "Tutorial";
                    }
                    else if (attendance.Note.Contains("Lecture", StringComparison.OrdinalIgnoreCase))
                    {
                        type = "Lecture";
                    }
                }

                var block = new CourseBlockDTO
                {
                    Id = $"block-{attendance.Id}-{dayName}-{startMinutes}",
                    Subject = course.Code,
                    Day = dayName,
                    StartMinutes = startMinutes,
                    EndMinutes = endMinutes,
                    Type = type
                };

                dayBlocks.Add((block, startMinutes, endMinutes));
            }

            // Resolve overlaps: ensure no two blocks overlap in time
            // Sort by start time
            dayBlocks = dayBlocks.OrderBy(b => b.StartMinutes).ToList();
            
            var resolvedBlocks = new List<CourseBlockDTO>();
            var occupiedSlots = new List<(int Start, int End)>(); // Track occupied time slots

            foreach (var (block, start, end) in dayBlocks)
            {
                // Check if this block overlaps with any existing block
                var overlaps = occupiedSlots.Any(slot => 
                    (start < slot.End && end > slot.Start)); // Standard overlap check

                if (!overlaps)
                {
                    // No overlap - add the block as-is
                    resolvedBlocks.Add(block);
                    occupiedSlots.Add((start, end));
                }
                // If overlaps, skip this block to avoid conflicts
                // In a real system, you might want to log this or handle it differently
            }

            blocks.AddRange(resolvedBlocks);
        }

        // Sort blocks by day and time for consistent display
        blocks = blocks
            .OrderBy(b => b.Day switch
            {
                "Mon" => 1,
                "Tue" => 2,
                "Wed" => 3,
                "Thu" => 4,
                "Fri" => 5,
                _ => 6
            })
            .ThenBy(b => b.StartMinutes)
            .ToList();

        // Determine start and end hours from blocks
        var startHour = blocks.Any() ? blocks.Min(b => b.StartMinutes) / 60 : 9;
        var endHour = blocks.Any() ? (int)Math.Ceiling(blocks.Max(b => b.EndMinutes) / 60.0) : 18;

        return new WeeklyScheduleDTO
        {
            StartHour = Math.Max(8, startHour - 1), // Add some padding
            EndHour = Math.Min(20, endHour + 1), // Add some padding
            Blocks = blocks
        };
    }

    public async Task<MonthlyCalendarDTO> GetMonthlyCalendarAsync(string userId, int year, int month, CancellationToken cancellationToken = default)
    {
        var student = await _studentExamRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student == null)
        {
            throw new InvalidOperationException($"Student with userId '{userId}' not found.");
        }

        // Create UTC dates to avoid PostgreSQL timezone issues
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

        // Get exams in this month
        var enrollments = await _context.Enrollments
            .Where(e => e.StudentId == student.Id)
            .Select(e => e.CourseId)
            .ToListAsync(cancellationToken);

        var exams = await _context.Exams
            .Where(e => enrollments.Contains(e.CourseId) &&
                       e.ExamDate.HasValue &&
                       e.ExamDate.Value >= startDate &&
                       e.ExamDate.Value <= endDate)
            .ToListAsync(cancellationToken);

        // Get assignments due in this month
        var assignments = await _context.Assignments
            .Where(a => enrollments.Contains(a.CourseId) &&
                       a.DueDate.HasValue &&
                       a.DueDate.Value >= startDate &&
                       a.DueDate.Value <= endDate)
            .ToListAsync(cancellationToken);

        var highlightedDays = new List<HighlightedDayDTO>();

        // Add exam days with event type and details
        foreach (var exam in exams)
        {
            if (exam.ExamDate.HasValue)
            {
                var course = await _context.Courses.FindAsync([exam.CourseId], cancellationToken);
                highlightedDays.Add(new HighlightedDayDTO
                {
                    Day = exam.ExamDate.Value.Day,
                    EventType = "Exam",
                    EventName = exam.Name ?? "Exam",
                    CourseCode = course?.Code
                });
            }
        }

        // Add assignment due dates with event type and details
        foreach (var assignment in assignments)
        {
            if (assignment.DueDate.HasValue)
            {
                var course = await _context.Courses.FindAsync([assignment.CourseId], cancellationToken);
                highlightedDays.Add(new HighlightedDayDTO
                {
                    Day = assignment.DueDate.Value.Day,
                    EventType = "Assignment",
                    EventName = assignment.Name,
                    CourseCode = course?.Code
                });
            }
        }

        // Remove duplicates (keep first occurrence - exams take priority)
        highlightedDays = highlightedDays
            .GroupBy(h => h.Day)
            .Select(g => g.OrderByDescending(h => h.EventType == "Exam").First())
            .ToList();

        return new MonthlyCalendarDTO
        {
            CurrentMonth = startDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            HighlightedDays = highlightedDays
        };
    }

    public async Task<StudentAttendanceStatsDTO> GetAttendanceStatsAsync(string userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var student = await _studentExamRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student == null)
        {
            throw new InvalidOperationException($"Student with userId '{userId}' not found.");
        }

        var course = await _context.Courses.FindAsync([courseId], cancellationToken);
        if (course == null)
        {
            throw new InvalidOperationException($"Course with id '{courseId}' not found.");
        }

        // Get all attendances for this course
        var attendances = await _context.Attendances
            .Where(a => a.StudentId == student.Id && a.CourseId == courseId)
            .ToListAsync(cancellationToken);

        // Separate lectures and tutorials based on Note field
        // Note field can contain "Tutorial" to mark it as a tutorial, otherwise it's a lecture
        // If Note is null/empty or doesn't contain "Tutorial", it's a lecture (default)
        // If Note contains "Tutorial", it's a tutorial
        var tutorials = attendances.Where(a => 
            !string.IsNullOrWhiteSpace(a.Note) && 
            a.Note.Contains("Tutorial", StringComparison.OrdinalIgnoreCase)
        ).ToList();

        // All other attendances (without "Tutorial" in Note) are lectures
        var lectures = attendances.Except(tutorials).ToList();

        // Calculate attendance statistics for lectures
        var lecturesPresent = lectures.Count(a => a.Status == "Present" || a.Status == "Late");
        var lecturesTotal = lectures.Count;
        var lecturesPercent = lecturesTotal > 0 ? (double)lecturesPresent / lecturesTotal * 100 : 0;

        // Calculate attendance statistics for tutorials
        var tutorialsPresent = tutorials.Count(a => a.Status == "Present" || a.Status == "Late");
        var tutorialsTotal = tutorials.Count;
        var tutorialsPercent = tutorialsTotal > 0 ? (double)tutorialsPresent / tutorialsTotal * 100 : 0;

        return new StudentAttendanceStatsDTO
        {
            ContextLabel = $"for {course.Code}",
            Items = new List<AttendanceItemDTO>
            {
                new AttendanceItemDTO
                {
                    Label = "Lectures",
                    Percent = Math.Round(lecturesPercent, 2),
                    PresentCount = lecturesPresent,
                    TotalCount = lecturesTotal
                },
                new AttendanceItemDTO
                {
                    Label = "Tutorials",
                    Percent = Math.Round(tutorialsPercent, 2),
                    PresentCount = tutorialsPresent,
                    TotalCount = tutorialsTotal
                }
            }
        };
    }

    public async Task<SubjectProgressDTO> GetSubjectProgressAsync(string userId, Guid? semesterId, CancellationToken cancellationToken = default)
    {
        var student = await _studentExamRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student == null)
        {
            throw new InvalidOperationException($"Student with userId '{userId}' not found.");
        }

        // Get enrollments - filter by semester if provided
        // Semester is determined by CreatedAt date:
        // - Winter semester: October to March (months 10-12, 1-3)
        // - Summer semester: April to September (months 4-9)
        var enrollmentsQuery = _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == student.Id);

        if (semesterId.HasValue)
        {
            // For now, we'll filter by academic year based on CreatedAt
            // This is a simplified approach - in a real system, you'd have a Semester entity
            // We'll use the semesterId as a year identifier (e.g., 2024 for 2024/2025 academic year)
            // For simplicity, we'll filter enrollments created in the last 2 years
            var currentYear = DateTime.UtcNow.Year;
            var targetYear = currentYear; // Default to current year
            
            // Try to parse semesterId as year (simplified approach)
            // In a real system, you'd query a Semester table
            enrollmentsQuery = enrollmentsQuery.Where(e => 
                e.CreatedAt.Year >= currentYear - 1 && 
                e.CreatedAt.Year <= currentYear
            );
        }

        var enrollments = await enrollmentsQuery.ToListAsync(cancellationToken);

        var items = new List<SubjectProgressItemDTO>();

        foreach (var enrollment in enrollments)
        {
            var course = enrollment.Course;
            if (course == null) continue;

            // Calculate progress based on attendance and assignments
            var attendances = await _context.Attendances
                .Where(a => a.StudentId == student.Id && a.CourseId == course.Id)
                .ToListAsync(cancellationToken);

            var studentAssignments = await _context.StudentAssignments
                .Where(sa => sa.StudentId == student.Id)
                .ToListAsync(cancellationToken);

            var assignmentIds = studentAssignments.Select(sa => sa.AssignmentId).ToList();
            var assignments = await _context.Assignments
                .Where(a => assignmentIds.Contains(a.Id) && a.CourseId == course.Id)
                .ToListAsync(cancellationToken);

            // Simple progress calculation: attendance percentage
            var totalAttendances = attendances.Count;
            var presentAttendances = attendances.Count(a => a.Status == "Present" || a.Status == "Late");
            var progressPercent = totalAttendances > 0 ? (double)presentAttendances / totalAttendances * 100 : 0;

            // Determine status based on enrollment and progress
            string status = "Enrolled";
            if (enrollment.Grade.HasValue)
            {
                status = enrollment.Grade.Value >= 6 ? "Passed" : "Failed";
            }
            else if (enrollment.Status != null)
            {
                status = enrollment.Status;
            }
            else if (progressPercent >= 80)
            {
                status = "In Progress";
            }
            else if (progressPercent > 0)
            {
                status = "In Progress";
            }

            items.Add(new SubjectProgressItemDTO
            {
                Code = course.Code,
                Percent = Math.Round(progressPercent, 2),
                Status = status
            });
        }

        return new SubjectProgressDTO
        {
            ContextLabel = semesterId.HasValue ? $"for semester {semesterId}" : "for current semester",
            Items = items
        };
    }
}

