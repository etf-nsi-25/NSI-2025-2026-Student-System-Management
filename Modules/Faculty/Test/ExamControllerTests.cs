using Faculty.API.Controllers;
using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Core.Entities;
using Faculty.Core.Enums;
using Faculty.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Faculty.Test;

public class ExamControllerTests
{
    private readonly Mock<IExamService> _examServiceMock;
    private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
    private readonly Mock<ILogger<ExamController>> _loggerMock;
    private readonly ExamController _controller;

    public ExamControllerTests()
    {
        _examServiceMock = new Mock<IExamService>();
        _teacherRepositoryMock = new Mock<ITeacherRepository>();
        _loggerMock = new Mock<ILogger<ExamController>>();
        _controller = new ExamController(
            _examServiceMock.Object,
            _teacherRepositoryMock.Object,
            _loggerMock.Object);

        // Setup controller context with claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Teacher"),
            new Claim("userId", "user123")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task CreateExam_ValidRequest_ShouldReturnCreatedResult()
    {
        // Arrange
        var teacherId = 1;
        var facultyId = Guid.NewGuid();
        var request = new CreateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Final Exam",
            Location = "Room 101",
            ExamType = ExamType.Written,
            ExamDate = DateTime.UtcNow.AddDays(7),
            RegDeadline = DateTime.UtcNow.AddDays(5)
        };

        var response = new ExamResponseDTO
        {
            Id = 1,
            CourseId = request.CourseId,
            Name = request.Name,
            Location = request.Location,
            ExamType = request.ExamType,
            ExamDate = request.ExamDate,
            RegDeadline = request.RegDeadline
        };

        var teacher = new Teacher { Id = teacherId, UserId = "user123", FacultyId = facultyId };

        _teacherRepositoryMock.Setup(x => x.GetByUserIdAsync("user123")).ReturnsAsync(teacher);
        _examServiceMock.Setup(x => x.CreateExamAsync(request, teacherId, facultyId)).ReturnsAsync(response);

        // Act
        var result = await _controller.CreateExam(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetExamById), createdResult.ActionName);
        Assert.Equal(response.Id, createdResult.RouteValues["id"]);
        Assert.Equal(response, createdResult.Value);
    }

    [Fact]
    public async Task CreateExam_InvalidModelState_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateExamRequestDTO(); // Empty request
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.CreateExam(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateExam_UnauthorizedRole_ShouldReturnForbid()
    {
        // Arrange - Setup controller with non-Teacher role
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Student"),
            new Claim("userId", "user123")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var request = new CreateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Final Exam"
        };

        // Act
        var result = await _controller.CreateExam(request);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task CreateExam_UnauthorizedAccessException_ShouldReturnForbid()
    {
        // Arrange
        var teacherId = 1;
        var facultyId = Guid.NewGuid();
        var request = new CreateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Final Exam"
        };

        var teacher = new Teacher { Id = teacherId, UserId = "user123", FacultyId = facultyId };

        _teacherRepositoryMock.Setup(x => x.GetByUserIdAsync("user123")).ReturnsAsync(teacher);
        _examServiceMock.Setup(x => x.CreateExamAsync(request, teacherId, facultyId))
            .ThrowsAsync(new UnauthorizedAccessException("Not authorized"));

        // Act
        var result = await _controller.CreateExam(request);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task CreateExam_Exception_ShouldReturnInternalServerError()
    {
        // Arrange
        var teacherId = 1;
        var facultyId = Guid.NewGuid();
        var request = new CreateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Final Exam"
        };

        var teacher = new Teacher { Id = teacherId, UserId = "user123", FacultyId = facultyId };

        _teacherRepositoryMock.Setup(x => x.GetByUserIdAsync("user123")).ReturnsAsync(teacher);
        _examServiceMock.Setup(x => x.CreateExamAsync(request, teacherId, facultyId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateExam(request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetExams_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var teacherId = 1;
        var exams = new List<ExamResponseDTO>
        {
            new ExamResponseDTO { Id = 1, Name = "Exam 1" },
            new ExamResponseDTO { Id = 2, Name = "Exam 2" }
        };

        var teacher = new Teacher { Id = teacherId, UserId = "user123" };

        _teacherRepositoryMock.Setup(x => x.GetByUserIdAsync("user123")).ReturnsAsync(teacher);
        _examServiceMock.Setup(x => x.GetExamsByTeacherAsync(teacherId)).ReturnsAsync(exams);

        // Act
        var result = await _controller.GetExams();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(exams, okResult.Value);
    }

    [Fact]
    public async Task GetExamById_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var examId = 1;
        var teacherId = 1;
        var exam = new ExamResponseDTO
        {
            Id = examId,
            Name = "Final Exam"
        };

        var teacher = new Teacher { Id = teacherId, UserId = "user123" };

        _teacherRepositoryMock.Setup(x => x.GetByUserIdAsync("user123")).ReturnsAsync(teacher);
        _examServiceMock.Setup(x => x.GetExamByIdAsync(examId, teacherId)).ReturnsAsync(exam);

        // Act
        var result = await _controller.GetExamById(examId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(exam, okResult.Value);
    }

    [Fact]
    public async Task GetExamById_ExamNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var examId = 999;
        var teacherId = 1;

        var teacher = new Teacher { Id = teacherId, UserId = "user123" };

        _teacherRepositoryMock.Setup(x => x.GetByUserIdAsync("user123")).ReturnsAsync(teacher);
        _examServiceMock.Setup(x => x.GetExamByIdAsync(examId, teacherId)).ReturnsAsync((ExamResponseDTO)null);

        // Act
        var result = await _controller.GetExamById(examId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateExam_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var examId = 1;
        var teacherId = 1;
        var request = new UpdateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Updated Exam",
            Location = "Room 202",
            ExamType = ExamType.Oral,
            ExamDate = DateTime.UtcNow.AddDays(10),
            RegDeadline = DateTime.UtcNow.AddDays(8)
        };

        var response = new ExamResponseDTO
        {
            Id = examId,
            Name = request.Name
        };

        var teacher = new Teacher { Id = teacherId, UserId = "user123" };

        _teacherRepositoryMock.Setup(x => x.GetByUserIdAsync("user123")).ReturnsAsync(teacher);
        _examServiceMock.Setup(x => x.UpdateExamAsync(examId, request, teacherId)).ReturnsAsync(response);

        // Act
        var result = await _controller.UpdateExam(examId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task UpdateExam_ExamNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var examId = 999;
        var teacherId = 1;
        var request = new UpdateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Updated Exam"
        };

        var teacher = new Teacher { Id = teacherId, UserId = "user123" };

        _teacherRepositoryMock.Setup(x => x.GetByUserIdAsync("user123")).ReturnsAsync(teacher);
        _examServiceMock.Setup(x => x.UpdateExamAsync(examId, request, teacherId)).ReturnsAsync((ExamResponseDTO)null);

        // Act
        var result = await _controller.UpdateExam(examId, request);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteExam_ValidRequest_ShouldReturnNoContent()
    {
        // Arrange
        var examId = 1;
        var teacherId = 1;

        var teacher = new Teacher { Id = teacherId, UserId = "user123" };

        _teacherRepositoryMock.Setup(x => x.GetByUserIdAsync("user123")).ReturnsAsync(teacher);
        _examServiceMock.Setup(x => x.DeleteExamAsync(examId, teacherId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteExam(examId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteExam_ExamNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var examId = 999;
        var teacherId = 1;

        var teacher = new Teacher { Id = teacherId, UserId = "user123" };

        _teacherRepositoryMock.Setup(x => x.GetByUserIdAsync("user123")).ReturnsAsync(teacher);
        _examServiceMock.Setup(x => x.DeleteExamAsync(examId, teacherId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteExam(examId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}