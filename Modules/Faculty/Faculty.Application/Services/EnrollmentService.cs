using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
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

    public async Task<EnrollmentResponseDto> CreateEnrollmentAsync(Guid courseId, string userId, CancellationToken cancellationToken = default)
    {
        if (courseId == Guid.Empty)
            throw new FacultyApplicationException("A valid course identifier must be provided.", HttpStatusCode.BadRequest);

        if (string.IsNullOrWhiteSpace(userId))
            throw new FacultyApplicationException("Authenticated user identifier was not provided.", HttpStatusCode.BadRequest);

        var student = await _repository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student == null)
            throw new FacultyApplicationException("Student record for the current user was not found.", HttpStatusCode.NotFound);

        var course = await _repository.GetCourseAsync(courseId, cancellationToken);
        if (course == null)
            throw new FacultyApplicationException("Requested course does not exist.", HttpStatusCode.NotFound);

        var alreadyEnrolled = await _repository.IsAlreadyEnrolledAsync(student.Id, courseId, cancellationToken);
        if (alreadyEnrolled)
            throw new FacultyApplicationException("Already enrolled.", HttpStatusCode.Conflict);

        var now = DateTime.UtcNow;

        var enrollment = new Enrollment
        {

            StudentId = student.Id,
            CourseId = course.Id,
            Status = DefaultStatus,
            CreatedAt = now
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
