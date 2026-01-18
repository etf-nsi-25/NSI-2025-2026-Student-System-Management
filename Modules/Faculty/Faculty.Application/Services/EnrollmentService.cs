using Faculty.Application.DTOs;
using Faculty.Application.Exceptions;
using Faculty.Application.Interfaces;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;

namespace Faculty.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private const string DefaultStatus = "Enrolled";
    private readonly IEnrollmentRepository _repository;

    public EnrollmentService(IEnrollmentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<List<StudentEnrollmentItemDto>> GetMyEnrollmentsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new FacultyApplicationException("Authenticated user identifier was not provided.");

        var student = await _repository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student == null)
            throw new FacultyApplicationException("Student record for the current user was not found.");

        var enrollments = await _repository.GetEnrollmentsByStudentIdAsync(student.Id, cancellationToken);

        return enrollments.Select(e => new StudentEnrollmentItemDto
        {
            EnrollmentId = e.Id,
            CourseId = e.CourseId,
            CourseName = e.Course?.Name ?? string.Empty,
            Status = e.Status,
            Grade = e.Grade
        }).ToList();
    }

    public async Task<EnrollmentResponseDto> CreateEnrollmentAsync(
        Guid courseId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (courseId == Guid.Empty)
            throw new FacultyApplicationException("A valid course identifier must be provided.");

        if (string.IsNullOrWhiteSpace(userId))
            throw new FacultyApplicationException("Authenticated user identifier was not provided.");

        var student = await _repository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student == null)
            throw new FacultyApplicationException("Student record for the current user was not found.");

        var course = await _repository.GetCourseAsync(courseId, cancellationToken);
        if (course == null)
            throw new FacultyApplicationException("Requested course does not exist.");

        var alreadyEnrolled = await _repository.IsAlreadyEnrolledAsync(student.Id, courseId, cancellationToken);
        if (alreadyEnrolled)
            throw new FacultyApplicationException("Already enrolled.");

        var enrollment = new Enrollment
        {
            StudentId = student.Id,
            CourseId = course.Id,
            Status = DefaultStatus,
            CreatedAt = DateTime.UtcNow
        };

        var saved = await _repository.CreateEnrollmentAsync(enrollment, cancellationToken);

        return new EnrollmentResponseDto
        {
            EnrollmentId = saved.Id,
            StudentId = saved.StudentId,
            CourseId = course.Id,
            CourseName = course.Name,
            EnrollmentDate = saved.CreatedAt
        };
    }
}
