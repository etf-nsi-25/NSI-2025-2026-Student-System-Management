using System.Net;
using Faculty.Application.Exceptions;
using Faculty.Application.Services;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Http;
using Faculty.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Faculty.Test;

public class StudentExamRegistrationServiceTests
{
    private readonly Guid _facultyId = Guid.NewGuid();

    [Fact]
    public async Task RegisterAsync_ShouldPersistRegistration_WhenStudentIsEligible()
    {
        await using var context = CreateContext();
        var userId = "student-eligible";
        var (student, course) = SeedStudentAndCourse(context, userId, includeEnrollment: true);
        var exam = SeedExam(context, course.Id, DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddDays(1));
        var service = CreateService(context);

        var result = await service.RegisterAsync(exam.Id, userId);

        Assert.Equal(exam.Id, result.ExamId);
        Assert.Equal(student.Id, result.StudentId);
        Assert.Equal("Registered", result.Status);
        Assert.Single(context.ExamRegistrations);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowForbidden_WhenStudentNotEnrolled()
    {
        await using var context = CreateContext();
        var userId = "student-not-enrolled";
        var (_, course) = SeedStudentAndCourse(context, userId, includeEnrollment: false);
        var exam = SeedExam(context, course.Id, DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddDays(1));
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<FacultyApplicationException>(() =>
            service.RegisterAsync(exam.Id, userId));

        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
    }

    [Fact]
    public async Task GetAvailableExamsAsync_ShouldReturnOnlyOpenAndUnregisteredExams()
    {
        await using var context = CreateContext();
        var userId = "student-filter";
        var (student, course) = SeedStudentAndCourse(context, userId, includeEnrollment: true);
        var visibleExam = SeedExam(context, course.Id, DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(3));
        var pastExam = SeedExam(context, course.Id, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(-2));
        var closedExam = SeedExam(context, course.Id, DateTime.UtcNow.AddDays(4), DateTime.UtcNow.AddHours(-1));
        var alreadyRegisteredExam = SeedExam(context, course.Id, DateTime.UtcNow.AddDays(6), DateTime.UtcNow.AddDays(4));

        context.ExamRegistrations.Add(new ExamRegistration
        {
            ExamId = alreadyRegisteredExam.Id,
            StudentId = student.Id,
            FacultyId = _facultyId,
            Status = "Registered",
            RegistrationDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var exams = await service.GetAvailableExamsAsync(userId);

        Assert.Single(exams);
        Assert.Equal(visibleExam.Id, exams[0].ExamId);
    }

    private StudentExamRegistrationService CreateService(FacultyDbContext context)
    {
        var repository = new StudentExamRegistrationRepository(context);
        return new StudentExamRegistrationService(repository);
    }

    private (Student student, Course course) SeedStudentAndCourse(FacultyDbContext context, string userId, bool includeEnrollment)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            FacultyId = _facultyId,
            Name = "Systems Programming",
            Code = "CS301",
            Type = CourseType.Mandatory,
            ProgramId = "CS",
            ECTS = 6,
            CreatedAt = DateTime.UtcNow
        };

        var student = new Student
        {
            FacultyId = _facultyId,
            UserId = userId,
            IndexNumber = "IB-123",
            FirstName = "Test",
            LastName = "Student",
            CreatedAt = DateTime.UtcNow
        };

        context.Courses.Add(course);
        context.Students.Add(student);

        if (includeEnrollment)
        {
            context.Enrollments.Add(new Enrollment
            {
                FacultyId = _facultyId,
                Student = student,
                Course = course,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            });
        }

        context.SaveChanges();
        return (student, course);
    }

    private Exam SeedExam(FacultyDbContext context, Guid courseId, DateTime examDate, DateTime? regDeadline)
    {
        var exam = new Exam
        {
            FacultyId = _facultyId,
            CourseId = courseId,
            Name = "Final Exam",
            ExamDate = examDate,
            RegDeadline = regDeadline,
            CreatedAt = DateTime.UtcNow
        };

        context.Exams.Add(exam);
        context.SaveChanges();
        return exam;
    }

    private FacultyDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FacultyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FacultyDbContext(options, new FixedTenantService(_facultyId));
    }

    private sealed class FixedTenantService : ITenantService
    {
        private readonly Guid _facultyId;

        public FixedTenantService(Guid facultyId)
        {
            _facultyId = facultyId;
        }

        public Guid GetCurrentFacultyId() => _facultyId;
    }
}
