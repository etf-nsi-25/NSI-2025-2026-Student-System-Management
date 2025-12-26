using Faculty.Application.DTOs;
using Faculty.Application.Services;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Faculty.Test;

public class ExamServiceTests
{
    private readonly Mock<IExamRepository> _examRepositoryMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly Mock<ILogger<ExamService>> _loggerMock;
    private readonly ExamService _examService;

    public ExamServiceTests()
    {
        _examRepositoryMock = new Mock<IExamRepository>();
        _tenantServiceMock = new Mock<ITenantService>();
        _loggerMock = new Mock<ILogger<ExamService>>();
        _examService = new ExamService(
            _examRepositoryMock.Object,
            _tenantServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateExamAsync_ValidRequest_ShouldCreateExam()
    {
        // Arrange
        var teacherId = 1;
        var courseId = Guid.NewGuid();
        var facultyId = Guid.NewGuid();
        var request = new CreateExamRequestDTO
        {
            CourseId = courseId,
            Name = "Final Exam",
            Location = "Room 101",
            ExamType = "Written",
            ExamDate = DateTime.UtcNow.AddDays(7),
            RegDeadline = DateTime.UtcNow.AddDays(5)
        };

        _tenantServiceMock.Setup(x => x.GetCurrentFacultyId()).Returns(facultyId);
        _examRepositoryMock.Setup(x => x.IsTeacherAssignedToCourseAsync(teacherId, courseId))
            .ReturnsAsync(true);

        var createdExam = new Exam
        {
            Id = 1,
            FacultyId = facultyId,
            CourseId = courseId,
            Name = request.Name,
            ExamDate = request.ExamDate,
            RegDeadline = request.RegDeadline,
            CreatedAt = DateTime.UtcNow
        };

        _examRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Exam>()))
            .ReturnsAsync(createdExam);

        // Act
        var result = await _examService.CreateExamAsync(request, teacherId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdExam.Id, result.Id);
        Assert.Equal(request.Name, result.Name);
        _examRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Exam>()), Times.Once);
    }

    [Fact]
    public async Task CreateExamAsync_TeacherNotAssignedToCourse_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var teacherId = 1;
        var courseId = Guid.NewGuid();
        var request = new CreateExamRequestDTO
        {
            CourseId = courseId,
            Name = "Final Exam",
            Location = "Room 101",
            ExamType = "Written",
            ExamDate = DateTime.UtcNow.AddDays(7),
            RegDeadline = DateTime.UtcNow.AddDays(5)
        };

            _examRepositoryMock.Setup(x => x.IsTeacherAssignedToCourseAsync(teacherId, courseId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _examService.CreateExamAsync(request, teacherId));
    }

    [Fact]
    public async Task GetExamByIdAsync_ValidRequest_ShouldReturnExam()
    {
        // Arrange
        var examId = 1;
        var teacherId = 1;
        var courseId = Guid.NewGuid();
        var exam = new Exam
        {
            Id = examId,
            CourseId = courseId,
            Name = "Final Exam",
            Course = new Course { Name = "Math 101" }
        };

        _examRepositoryMock.Setup(x => x.GetByIdAsync(examId)).ReturnsAsync(exam);
            _examRepositoryMock.Setup(x => x.IsTeacherAssignedToCourseAsync(teacherId, courseId))
            .ReturnsAsync(true);

        // Act
        var result = await _examService.GetExamByIdAsync(examId, teacherId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(examId, result.Id);
        Assert.Equal("Final Exam", result.Name);
    }

    [Fact]
    public async Task GetExamByIdAsync_TeacherNotAssignedToCourse_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var examId = 1;
        var teacherId = 1;
        var courseId = Guid.NewGuid();
        var exam = new Exam
        {
            Id = examId,
            CourseId = courseId,
            Name = "Final Exam"
        };

        _examRepositoryMock.Setup(x => x.GetByIdAsync(examId)).ReturnsAsync(exam);
            _examRepositoryMock.Setup(x => x.IsTeacherAssignedToCourseAsync(teacherId, courseId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _examService.GetExamByIdAsync(examId, teacherId));
    }

    [Fact]
    public async Task GetExamsByTeacherAsync_ShouldReturnExams()
    {
        // Arrange
        var teacherId = 1;
        var exams = new List<Exam>
        {
            new Exam
            {
                Id = 1,
                CourseId = Guid.NewGuid(),
                Name = "Exam 1",
                Course = new Course { Name = "Math 101" }
            },
            new Exam
            {
                Id = 2,
                CourseId = Guid.NewGuid(),
                Name = "Exam 2",
                Course = new Course { Name = "Physics 101" }
            }
        };

        _examRepositoryMock.Setup(x => x.GetExamsByTeacherAsync(teacherId))
            .ReturnsAsync(exams);

        // Act
        var result = await _examService.GetExamsByTeacherAsync(teacherId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Exam 1", result[0].Name);
        Assert.Equal("Exam 2", result[1].Name);
    }

    [Fact]
    public async Task UpdateExamAsync_ValidRequest_ShouldUpdateExam()
    {
        // Arrange
        var examId = 1;
        var teacherId = 1;
        var courseId = Guid.NewGuid();
        var existingExam = new Exam
        {
            Id = examId,
            CourseId = courseId,
            Name = "Old Exam",
            Location = "Room 101",
            ExamType = "Written",
            ExamDate = DateTime.UtcNow.AddDays(7),
            RegDeadline = DateTime.UtcNow.AddDays(5)
        };

        var request = new UpdateExamRequestDTO
        {
            CourseId = courseId,
            Name = "Updated Exam",
            Location = "Room 202",
            ExamType = "Oral",
            ExamDate = DateTime.UtcNow.AddDays(10),
            RegDeadline = DateTime.UtcNow.AddDays(8)
        };

        _examRepositoryMock.Setup(x => x.GetByIdAsync(examId)).ReturnsAsync(existingExam);
            _examRepositoryMock.Setup(x => x.IsTeacherAssignedToCourseAsync(teacherId, courseId))
            .ReturnsAsync(true);
        _examRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Exam>()))
            .ReturnsAsync(existingExam);

        // Act
        var result = await _examService.UpdateExamAsync(examId, request, teacherId);

        // Assert
        Assert.NotNull(result);
        _examRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Exam>(e =>
            e.Name == request.Name &&
            e.ExamDate == request.ExamDate &&
            e.RegDeadline == request.RegDeadline)), Times.Once);
    }

    [Fact]
    public async Task DeleteExamAsync_ValidRequest_ShouldDeleteExam()
    {
        // Arrange
        var examId = 1;
        var teacherId = 1;
        var courseId = Guid.NewGuid();
        var exam = new Exam
        {
            Id = examId,
            CourseId = courseId,
            Name = "Exam to Delete"
        };

        _examRepositoryMock.Setup(x => x.GetByIdAsync(examId)).ReturnsAsync(exam);
            _examRepositoryMock.Setup(x => x.IsTeacherAssignedToCourseAsync(teacherId, courseId))
            .ReturnsAsync(true);
        _examRepositoryMock.Setup(x => x.DeleteAsync(examId)).ReturnsAsync(true);

        // Act
        var result = await _examService.DeleteExamAsync(examId, teacherId);

        // Assert
        Assert.True(result);
        _examRepositoryMock.Verify(x => x.DeleteAsync(examId), Times.Once);
    }

    [Fact]
    public async Task DeleteExamAsync_TeacherNotAssignedToCourse_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var examId = 1;
        var teacherId = 1;
        var courseId = Guid.NewGuid();
        var exam = new Exam
        {
            Id = examId,
            CourseId = courseId,
            Name = "Exam to Delete"
        };

        _examRepositoryMock.Setup(x => x.GetByIdAsync(examId)).ReturnsAsync(exam);
            _examRepositoryMock.Setup(x => x.IsTeacherAssignedToCourseAsync(teacherId, courseId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _examService.DeleteExamAsync(examId, teacherId));
    }

    [Fact]
    public async Task CreateExamAsync_DateConflict_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var teacherId = 1;
        var courseId = Guid.NewGuid();
        var facultyId = Guid.NewGuid();
        var examDate = DateTime.UtcNow.AddDays(7);
        var location = "Room 101";

        var request = new CreateExamRequestDTO
        {
            CourseId = courseId,
            Name = "Final Exam",
            Location = location,
            ExamType = "Written",
            ExamDate = examDate,
            RegDeadline = DateTime.UtcNow.AddDays(5)
        };

        _tenantServiceMock.Setup(x => x.GetCurrentFacultyId()).Returns(facultyId);
            _examRepositoryMock.Setup(x => x.IsTeacherAssignedToCourseAsync(teacherId, courseId))
            .ReturnsAsync(true);
        _examRepositoryMock.Setup(x => x.HasDateConflictAsync(courseId, null, examDate, location))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _examService.CreateExamAsync(request, teacherId));
    }

    [Fact]
    public async Task UpdateExamAsync_DateConflict_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var examId = 1;
        var teacherId = 1;
        var courseId = Guid.NewGuid();
        var examDate = DateTime.UtcNow.AddDays(10);
        var location = "Room 202";

        var existingExam = new Exam
        {
            Id = examId,
            CourseId = courseId,
            Name = "Old Exam",
            Location = "Room 101",
            ExamType = "Written",
            ExamDate = DateTime.UtcNow.AddDays(7),
            RegDeadline = DateTime.UtcNow.AddDays(5)
        };

        var request = new UpdateExamRequestDTO
        {
            CourseId = courseId,
            Name = "Updated Exam",
            Location = location,
            ExamType = "Oral",
            ExamDate = examDate,
            RegDeadline = DateTime.UtcNow.AddDays(8)
        };

        _examRepositoryMock.Setup(x => x.GetByIdAsync(examId)).ReturnsAsync(existingExam);
            _examRepositoryMock.Setup(x => x.IsTeacherAssignedToCourseAsync(teacherId, courseId))
            .ReturnsAsync(true);
        _examRepositoryMock.Setup(x => x.HasDateConflictAsync(courseId, examId, examDate, location))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _examService.UpdateExamAsync(examId, request, teacherId));
    }

    [Fact]
    public async Task UpdateExamAsync_ExamNotFound_ShouldReturnNull()
    {
        // Arrange
        var examId = 999;
        var teacherId = 1;
        var request = new UpdateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Updated Exam",
            Location = "Room 202",
            ExamType = "Oral",
            ExamDate = DateTime.UtcNow.AddDays(10),
            RegDeadline = DateTime.UtcNow.AddDays(8)
        };

        _examRepositoryMock.Setup(x => x.GetByIdAsync(examId)).ReturnsAsync((Exam)null);

        // Act
        var result = await _examService.UpdateExamAsync(examId, request, teacherId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteExamAsync_ExamNotFound_ShouldReturnFalse()
    {
        // Arrange
        var examId = 999;
        var teacherId = 1;

        _examRepositoryMock.Setup(x => x.GetByIdAsync(examId)).ReturnsAsync((Exam)null);

        // Act
        var result = await _examService.DeleteExamAsync(examId, teacherId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetExamByIdAsync_ExamNotFound_ShouldReturnNull()
    {
        // Arrange
        var examId = 999;
        var teacherId = 1;

        _examRepositoryMock.Setup(x => x.GetByIdAsync(examId)).ReturnsAsync((Exam)null);

        // Act
        var result = await _examService.GetExamByIdAsync(examId, teacherId);

        // Assert
        Assert.Null(result);
    }
}