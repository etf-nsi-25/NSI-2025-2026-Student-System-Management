using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Core.Interfaces;

namespace Faculty.Application.Services;

/// <summary>
/// Service for retrieving upcoming activities for professors.
/// </summary>
public class UpcomingActivityService : IUpcomingActivityService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IExamRepository _examRepository;

    public UpcomingActivityService(
        ICourseRepository courseRepository,
        IAssignmentRepository assignmentRepository,
        IExamRepository examRepository)
    {
        _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        _examRepository = examRepository ?? throw new ArgumentNullException(nameof(examRepository));
    }

    public async Task<List<UpcomingActivityDTO>> GetUpcomingActivitiesAsync(string userId)
    {
        var courses = await _courseRepository.GetProfessorCoursesWithStudentCountAsync(userId);
        var courseIds = courses.Select(c => c.Course.Id).ToList();

        var assignments = await _assignmentRepository.GetUpcomingByCourseIdsAsync(courseIds);
        var exams = await _examRepository.GetUpcomingByCourseIdsAsync(courseIds);

        var activities = new List<UpcomingActivityDTO>();

        foreach (var assignment in assignments)
        {
            if (assignment.DueDate.HasValue)
            {
                activities.Add(new UpcomingActivityDTO
                {
                    Date = assignment.DueDate.Value.Date,
                    Time = assignment.DueDate.Value.TimeOfDay,
                    Title = assignment.Name,
                    Type = null,
                    Location = null,
                    CourseId = assignment.CourseId,
                    CourseName = assignment.Course?.Name ?? string.Empty
                });
            }
        }

        foreach (var exam in exams)
        {
            if (exam.ExamDate.HasValue)
            {
                activities.Add(new UpcomingActivityDTO
                {
                    Date = exam.ExamDate.Value.Date,
                    Time = exam.ExamDate.Value.TimeOfDay,
                    Title = exam.Name,
                    Type = exam.ExamType.ToString(),
                    Location = exam.Location,
                    CourseId = exam.CourseId,
                    CourseName = exam.Course?.Name ?? string.Empty
                });
            }
        }

        return activities.OrderBy(a => a.Date).ThenBy(a => a.Time).ToList();
    }
}
