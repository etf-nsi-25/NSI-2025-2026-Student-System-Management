using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Schemas;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Services;

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

        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == student.Id)
            .ToListAsync(cancellationToken);

        var passedEnrollments = enrollments.Where(e => e.Grade.HasValue && e.Grade >= 6).ToList();
        var totalEnrollments = enrollments.Count;

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

        var startDateUtc = startDate.Kind == DateTimeKind.Utc 
            ? startDate 
            : DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        
        var startOfWeek = startDateUtc.Date;
        while (startOfWeek.DayOfWeek != DayOfWeek.Monday)
        {
            startOfWeek = startOfWeek.AddDays(-1);
        }
        var endOfWeek = startOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == student.Id)
            .ToListAsync(cancellationToken);

        var courseIds = enrollments.Select(e => e.CourseId).ToList();

        var attendances = await _context.Attendances
            .Include(a => a.Course)
            .Where(a => a.StudentId == student.Id &&
                       courseIds.Contains(a.CourseId) &&
                       a.LectureDate >= startOfWeek &&
                       a.LectureDate <= endOfWeek)
            .OrderBy(a => a.LectureDate)
            .ToListAsync(cancellationToken);

        var blocks = new List<CourseBlockDTO>();

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

            if (dayName == null) continue;

            var dayAttendances = dayGroup.Value.OrderBy(a => a.LectureDate).ToList();
            var dayBlocks = new List<(CourseBlockDTO Block, int StartMinutes, int EndMinutes)>();

            for (int i = 0; i < dayAttendances.Count; i++)
            {
                var attendance = dayAttendances[i];
                var course = await _context.Courses.FindAsync([attendance.CourseId], cancellationToken);
                if (course == null) continue;

                var attendanceDate = attendance.LectureDate;
                var startMinutes = attendanceDate.Hour * 60 + attendanceDate.Minute;

                int endMinutes;
                
                var nextSameCourse = dayAttendances
                    .Skip(i + 1)
                    .FirstOrDefault(a => a.CourseId == attendance.CourseId);
                
                if (nextSameCourse != null)
                {
                    var nextStartMinutes = nextSameCourse.LectureDate.Hour * 60 + nextSameCourse.LectureDate.Minute;
                    var duration = nextStartMinutes - startMinutes;
                    duration = Math.Max(30, Math.Min(240, duration));
                    endMinutes = startMinutes + duration;
                }
                else
                {
                    var nextAnyCourse = dayAttendances.Skip(i + 1).FirstOrDefault();
                    if (nextAnyCourse != null)
                    {
                        var nextStartMinutes = nextAnyCourse.LectureDate.Hour * 60 + nextAnyCourse.LectureDate.Minute;
                        var duration = nextStartMinutes - startMinutes;
                        duration = Math.Max(30, Math.Min(240, duration));
                        endMinutes = startMinutes + duration;
                    }
                    else
                    {
                        var isTutorial = !string.IsNullOrWhiteSpace(attendance.Note) && 
                                       attendance.Note.Contains("Tutorial", StringComparison.OrdinalIgnoreCase);
                        var defaultDuration = isTutorial ? 60 : 90;
                        endMinutes = startMinutes + defaultDuration;
                    }
                }

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

            dayBlocks = dayBlocks.OrderBy(b => b.StartMinutes).ToList();
            
            var resolvedBlocks = new List<CourseBlockDTO>();
            var occupiedSlots = new List<(int Start, int End)>();

            foreach (var (block, start, end) in dayBlocks)
            {
                var overlaps = occupiedSlots.Any(slot => 
                    (start < slot.End && end > slot.Start));

                if (!overlaps)
                {
                    resolvedBlocks.Add(block);
                    occupiedSlots.Add((start, end));
                }
            }

            blocks.AddRange(resolvedBlocks);
        }

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

        var startHour = blocks.Any() ? blocks.Min(b => b.StartMinutes) / 60 : 9;
        var endHour = blocks.Any() ? (int)Math.Ceiling(blocks.Max(b => b.EndMinutes) / 60.0) : 18;

        return new WeeklyScheduleDTO
        {
            StartHour = Math.Max(8, startHour - 1),
            EndHour = Math.Min(20, endHour + 1),
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

        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

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

        var assignments = await _context.Assignments
            .Where(a => enrollments.Contains(a.CourseId) &&
                       a.DueDate.HasValue &&
                       a.DueDate.Value >= startDate &&
                       a.DueDate.Value <= endDate)
            .ToListAsync(cancellationToken);

        var highlightedDays = new List<HighlightedDayDTO>();

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

        var attendances = await _context.Attendances
            .Where(a => a.StudentId == student.Id && a.CourseId == courseId)
            .ToListAsync(cancellationToken);

        var tutorials = attendances.Where(a => 
            !string.IsNullOrWhiteSpace(a.Note) && 
            a.Note.Contains("Tutorial", StringComparison.OrdinalIgnoreCase)
        ).ToList();

        var lectures = attendances.Except(tutorials).ToList();

        var lecturesPresent = lectures.Count(a => a.Status == "Present" || a.Status == "Late");
        var lecturesTotal = lectures.Count;
        var lecturesPercent = lecturesTotal > 0 ? (double)lecturesPresent / lecturesTotal * 100 : 0;

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

        var enrollmentsQuery = _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == student.Id);

        if (semesterId.HasValue)
        {
            var currentYear = DateTime.UtcNow.Year;
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

            var totalAttendances = attendances.Count;
            var presentAttendances = attendances.Count(a => a.Status == "Present" || a.Status == "Late");
            var progressPercent = totalAttendances > 0 ? (double)presentAttendances / totalAttendances * 100 : 0;

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

