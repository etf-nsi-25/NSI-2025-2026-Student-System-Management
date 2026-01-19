using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Application.Exceptions;
using Faculty.Core.Interfaces;
using System.Net;

namespace Faculty.Application.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _repository;
    private readonly IStudentRepository _studentRepository;

    public AssignmentService(IAssignmentRepository repository, IStudentRepository studentRepository)
    {
        _repository = repository;
        _studentRepository = studentRepository;
    }

    public async Task<List<StudentAssignmentOverviewDTO>> GetStudentAssignmentsForCourseAsync(Guid courseId, string userId)
{
    var student = await _studentRepository.GetByUserIdAsync(userId);
    if (student == null) throw new FacultyApplicationException("Student not found", HttpStatusCode.NotFound);

    bool isEnrolled = await _repository.IsStudentEnrolledAsync(student.Id, courseId);
    if (!isEnrolled) throw new FacultyApplicationException("Student is not enrolled in this course", HttpStatusCode.Forbidden);

    var assignments = await _repository.GetAssignmentsWithSubmissionsAsync(courseId, student.Id);
    var now = DateTime.UtcNow;

    return assignments
        .OrderBy(a => a.DueDate) 
        .Select(a => {
            var submission = a.StudentAssignments.FirstOrDefault(); 
            
            return new StudentAssignmentOverviewDTO {
                AssignmentId = a.Id,
                Title = a.Name,
                Description = a.Description,
                DueDate = a.DueDate,
                MaxPoints = a.MaxPoints, 
                SubmissionDate = submission?.SubmissionDate,
                Grade = submission?.Grade,
                Points = submission?.Points,
                Feedback = submission?.Feedback,
                Status = CalculateStatus(submission, a.DueDate, now)
            };
        }).ToList();
}

    private string CalculateStatus(Faculty.Core.Entities.StudentAssignment? sub, DateTime? dueDate, DateTime now)
    {
        if (sub != null)
        {
            if (sub.Grade.HasValue || sub.Points.HasValue) return "Graded";
            return "Submitted";
        }
        
        if (dueDate.HasValue && now > dueDate.Value) return "Missed";
        return "Pending";
    }
}