using System.Net;
using Faculty.Application.DTOs;
using Faculty.Application.Exceptions;
using Faculty.Application.Interfaces;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;

namespace Faculty.Application.Services;

public class StudentExamRegistrationService : IStudentExamRegistrationService
{
    private const string DefaultStatus = "Registered";
    private readonly IStudentExamRegistrationRepository _repository;

    public StudentExamRegistrationService(IStudentExamRegistrationRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ExamRegistrationResponseDto> RegisterAsync(int examId, string userId, CancellationToken cancellationToken = default)
    {
        if (examId <= 0)
        {
            throw new FacultyApplicationException("A valid exam identifier must be provided.", HttpStatusCode.BadRequest);
        }

        ValidateIdentity(userId);

        var student = await ResolveStudentAsync(userId, cancellationToken);
        var exam = await _repository.GetExamWithDetailsAsync(examId, cancellationToken)
                   ?? throw new FacultyApplicationException("Requested exam does not exist.", HttpStatusCode.NotFound);

        var now = DateTime.UtcNow;

        if (!exam.ExamDate.HasValue || exam.ExamDate.Value <= now)
        {
            throw new FacultyApplicationException("Cannot register for an exam that has already concluded.", HttpStatusCode.BadRequest);
        }

        if (exam.RegDeadline.HasValue && exam.RegDeadline.Value < now)
        {
            throw new FacultyApplicationException("Registration window for this exam is closed.", HttpStatusCode.BadRequest);
        }

        var enrolledCourseIds = student.Enrollments.Select(e => e.CourseId).ToHashSet();
        if (!enrolledCourseIds.Contains(exam.CourseId))
        {
            throw new FacultyApplicationException("You are not enrolled in the course for this exam.", HttpStatusCode.Forbidden);
        }

        var alreadyRegistered = await _repository.HasExistingRegistrationAsync(student.Id, exam.Id, cancellationToken);
        if (alreadyRegistered)
        {
            throw new FacultyApplicationException("You are already registered for this exam.", HttpStatusCode.Conflict);
        }

        var registration = new ExamRegistration
        {
            StudentId = student.Id,
            ExamId = exam.Id,
            FacultyId = student.FacultyId,
            RegistrationDate = now,
            CreatedAt = now,
            Status = DefaultStatus
        };

        var saved = await _repository.SaveRegistrationAsync(registration, cancellationToken);

        return new ExamRegistrationResponseDto
        {
            RegistrationId = saved.Id,
            ExamId = saved.ExamId,
            StudentId = saved.StudentId,
            RegistrationDate = saved.RegistrationDate,
            Status = saved.Status,
            ExamDate = exam.ExamDate,
            ExamName = ResolveExamName(exam)
        };
    }

    public async Task<IReadOnlyList<AvailableExamDto>> GetAvailableExamsAsync(string userId, CancellationToken cancellationToken = default)
    {
        ValidateIdentity(userId);
        var student = await ResolveStudentAsync(userId, cancellationToken);

        var enrolledCourseIds = student.Enrollments
            .Select(e => e.CourseId)
            .Distinct()
            .ToArray();

        var available = await _repository.GetEligibleExamsAsync(student.Id, enrolledCourseIds, DateTime.UtcNow, cancellationToken);

        return available.Select(exam => new AvailableExamDto
        {
            ExamId = exam.Id,
            CourseId = exam.CourseId,
            CourseCode = exam.Course?.Code ?? string.Empty,
            CourseName = exam.Course?.Name ?? string.Empty,
            ExamName = ResolveExamName(exam),
            ExamDate = exam.ExamDate!.Value,
            RegistrationDeadline = exam.RegDeadline
        }).ToList();
    }

    public async Task<IReadOnlyList<RegisteredExamDto>> GetRegistrationsAsync(string userId, CancellationToken cancellationToken = default)
    {
        ValidateIdentity(userId);
        var student = await ResolveStudentAsync(userId, cancellationToken);

        var registrations = await _repository.GetRegistrationsAsync(student.Id, cancellationToken);

        return registrations.Select(registration => new RegisteredExamDto
        {
            RegistrationId = registration.Id,
            ExamId = registration.ExamId,
            CourseId = registration.Exam.CourseId,
            CourseCode = registration.Exam.Course?.Code ?? string.Empty,
            CourseName = registration.Exam.Course?.Name ?? string.Empty,
            ExamName = ResolveExamName(registration.Exam),
            ExamDate = registration.Exam.ExamDate,
            RegistrationDate = registration.RegistrationDate,
            Status = registration.Status
        }).ToList();
    }

    private async Task<Student> ResolveStudentAsync(string userId, CancellationToken cancellationToken)
    {
        var student = await _repository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student == null)
        {
            throw new FacultyApplicationException("Student record for the current user was not found.", HttpStatusCode.NotFound);
        }

        return student;
    }

    private static void ValidateIdentity(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new FacultyApplicationException("Authenticated user identifier was not provided.", HttpStatusCode.BadRequest);
        }
    }

    private static string ResolveExamName(Exam exam)
    {
        if (!string.IsNullOrWhiteSpace(exam.Name))
        {
            return exam.Name;
        }

        return exam.Course?.Name ?? "Exam";
    }
}
