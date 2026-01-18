using Faculty.Application.DTOs;
using Faculty.Application.Services;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Faculty.Tests;

public class StudentExamGradeServiceTests
{
    private readonly Mock<IStudentExamGradeRepository> _repo = new();
    private readonly Mock<ILogger<StudentExamGradeService>> _logger = new();

    private readonly int _teacherId = 42;

    private StudentExamGradeService CreateService()
        => new(_repo.Object, _logger.Object);

    [Fact]
    public async Task CreateGrade_Success()
    {
        _repo.Setup(r => r.ExamBelongsToTeacherAsync(1, _teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _repo.Setup(r => r.GetAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentExamGrade?)null);

        _repo.Setup(r => r.CreateAsync(It.IsAny<StudentExamGrade>(), It.IsAny<CancellationToken>()))
            .Callback<StudentExamGrade, CancellationToken>((g, _) =>
                g.Student = new Student { FirstName = "John", LastName = "Doe" })
            .ReturnsAsync((StudentExamGrade g, CancellationToken _) => g);

        var service = CreateService();

        var result = await service.CreateOrUpdateAsync(
            examId: 1,
            studentId: 1,
            new GradeRequestDTO
            {
                Points = 85,
                Passed = true,
                Url = "https://example.com"
            },
            _teacherId,
            default);

        _repo.Verify(
            r => r.CreateAsync(It.Is<StudentExamGrade>(g => g.StudentId == 1 && g.Points == 85), default),
            Times.Once);

        Assert.Equal("John Doe", result.StudentName);
    }

    [Fact]
    public async Task UpdateGrade_Success()
    {
        _repo.Setup(r => r.ExamBelongsToTeacherAsync(1, _teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _repo.Setup(r => r.GetAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StudentExamGrade
            {
                StudentId = 1,
                ExamId = 1,
                Points = 60,
                Passed = true,
                DateRecorded = DateTime.UtcNow,
                Student = new Student { FirstName = "John", LastName = "Doe" }
            });

        _repo.Setup(r => r.UpdateAsync(It.IsAny<StudentExamGrade>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentExamGrade g, CancellationToken _) => g);

        var service = CreateService();

        var result = await service.CreateOrUpdateAsync(
            examId: 1,
            studentId: 1,
            new GradeRequestDTO
            {
                Points = 70,
                Passed = true,
                Url = "https://example.com"
            },
            _teacherId,
            default);

        _repo.Verify(
            r => r.UpdateAsync(It.Is<StudentExamGrade>(g => g.Points == 70), default),
            Times.Once);

        Assert.Equal("John Doe", result.StudentName);
    }

    [Fact]
    public async Task DeleteGrade_Success()
    {
        var grade = new StudentExamGrade
        {
            StudentId = 1,
            ExamId = 1,
            Student = new Student { FirstName = "John", LastName = "Doe" }
        };

        _repo.Setup(r => r.GetAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(grade);

        _repo.Setup(r => r.ExamBelongsToTeacherAsync(1, _teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService();

        await service.DeleteAsync(1, 1, _teacherId, default);

        _repo.Verify(r => r.DeleteAsync(grade, default), Times.Once);
    }

    [Fact]
    public async Task UnauthorizedAccess_Throws()
    {
        _repo.Setup(r => r.ExamBelongsToTeacherAsync(1, _teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.CreateOrUpdateAsync(
                1,
                1,
                new GradeRequestDTO { Points = 80, Passed = true },
                _teacherId,
                default));
    }

    [Fact]
    public async Task InvalidPoints_Throws()
    {
        _repo.Setup(r => r.ExamBelongsToTeacherAsync(1, _teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateOrUpdateAsync(
                1,
                1,
                new GradeRequestDTO { Points = 120, Passed = true },
                _teacherId,
                default));
    }

    [Fact]
    public async Task GetAllForExam_ReturnsGrades()
    {
        var grades = new List<StudentExamGrade>
        {
            new()
            {
                StudentId = 1,
                Points = 80,
                Passed = true,
                Student = new Student { FirstName = "John", LastName = "Doe" }
            },
            new()
            {
                StudentId = 2,
                Points = 50,
                Passed = false,
                Student = new Student { FirstName = "Jane", LastName = "Smith" }
            }
        };

        _repo.Setup(r => r.ExamBelongsToTeacherAsync(1, _teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _repo.Setup(r => r.GetAllForExamAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(grades);

        var service = CreateService();

        var result = await service.GetAllForExamAsync(1, _teacherId, default);

        Assert.Equal(2, result.Grades.Count());
        Assert.Contains(result.Grades, g => g.StudentName == "John Doe");
        Assert.Contains(result.Grades, g => g.StudentName == "Jane Smith");
    }

    [Fact]
    public async Task UpdateComponents_UpdatesPointsAndPassed()
    {
        var grade = new StudentExamGrade
        {
            StudentId = 1,
            ExamId = 1,
            Points = 60,
            Passed = true,
            DateRecorded = DateTime.UtcNow.AddDays(-1),
            Student = new Student { FirstName = "John", LastName = "Doe" }
        };

        _repo.Setup(r => r.ExamBelongsToTeacherAsync(1, _teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _repo.Setup(r => r.GetAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(grade);

        _repo.Setup(r => r.UpdateAsync(It.IsAny<StudentExamGrade>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentExamGrade g, CancellationToken _) => g);

        var service = CreateService();

        var dto = new GradeUpdateRequestDTO
        {
            Points = 40,
            DateRecorded = DateTime.UtcNow
        };

        var result = await service.UpdateAsync(1, 1, dto, _teacherId, default);

        _repo.Verify(
            r => r.UpdateAsync(It.Is<StudentExamGrade>(
                g => g.Points == 40 &&
                     g.Passed == false &&
                     g.DateRecorded == dto.DateRecorded), default),
            Times.Once);

        Assert.Equal("John Doe", result.StudentName);
    }
}