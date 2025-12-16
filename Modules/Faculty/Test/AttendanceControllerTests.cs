using Faculty.API.Controllers;
using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Faculty.Test;

/// <summary>
/// Unit tests for AttendanceController covering happy paths, validation errors, and authorization scenarios.
/// </summary>
public class AttendanceControllerTests
{
    private readonly Mock<IAttendanceService> _mockAttendanceService;
    private readonly AttendanceController _controller;
    private readonly Guid _testCourseId = Guid.NewGuid();
    private readonly DateTime _testDate = new DateTime(2024, 1, 15);
    private readonly string _testUserId = Guid.NewGuid().ToString();

    public AttendanceControllerTests()
    {
        _mockAttendanceService = new Mock<IAttendanceService>();
        _controller = new AttendanceController(_mockAttendanceService.Object);
        
        // Set up authenticated user context
        SetupAuthenticatedUser();
    }

    private void SetupAuthenticatedUser()
    {
        var claims = new List<Claim>
        {
            new Claim("userId", _testUserId),
            new Claim(ClaimTypes.Name, "testuser")
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    #region GetStudentsWithAttendance Tests

    [Fact]
    public async Task GetStudentsWithAttendance_ShouldReturnOk_WhenAuthorized()
    {
        // Arrange
        var expectedStudents = new List<StudentAttendanceDTO>
        {
            new StudentAttendanceDTO
            {
                StudentId = 1,
                IndexNumber = "A-001",
                FirstName = "John",
                LastName = "Doe",
                Status = "Present",
                Note = "On time"
            },
            new StudentAttendanceDTO
            {
                StudentId = 2,
                IndexNumber = "A-002",
                FirstName = "Jane",
                LastName = "Smith",
                Status = "Absent",
                Note = null
            }
        };

        _mockAttendanceService
            .Setup(s => s.GetStudentsWithAttendanceAsync(_testCourseId, _testDate, _testUserId))
            .ReturnsAsync(expectedStudents);

        // Act
        var result = await _controller.GetStudentsWithAttendance(_testCourseId, _testDate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStudents = Assert.IsAssignableFrom<List<StudentAttendanceDTO>>(okResult.Value);
        Assert.Equal(2, returnedStudents.Count);
        Assert.Equal("A-001", returnedStudents[0].IndexNumber);
        Assert.Equal("Present", returnedStudents[0].Status);
        _mockAttendanceService.Verify(s => s.GetStudentsWithAttendanceAsync(_testCourseId, _testDate, _testUserId), Times.Once);
    }

    [Fact]
    public async Task GetStudentsWithAttendance_ShouldReturnOk_WhenEmptyList()
    {
        // Arrange
        var expectedStudents = new List<StudentAttendanceDTO>();

        _mockAttendanceService
            .Setup(s => s.GetStudentsWithAttendanceAsync(_testCourseId, _testDate, _testUserId))
            .ReturnsAsync(expectedStudents);

        // Act
        var result = await _controller.GetStudentsWithAttendance(_testCourseId, _testDate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStudents = Assert.IsAssignableFrom<List<StudentAttendanceDTO>>(okResult.Value);
        Assert.Empty(returnedStudents);
    }

    [Fact]
    public async Task GetStudentsWithAttendance_ShouldReturnOk_WhenNoAttendanceRecords()
    {
        // Arrange
        var expectedStudents = new List<StudentAttendanceDTO>
        {
            new StudentAttendanceDTO
            {
                StudentId = 1,
                IndexNumber = "A-001",
                FirstName = "John",
                LastName = "Doe",
                Status = null, // No attendance recorded yet
                Note = null
            }
        };

        _mockAttendanceService
            .Setup(s => s.GetStudentsWithAttendanceAsync(_testCourseId, _testDate, _testUserId))
            .ReturnsAsync(expectedStudents);

        // Act
        var result = await _controller.GetStudentsWithAttendance(_testCourseId, _testDate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStudents = Assert.IsAssignableFrom<List<StudentAttendanceDTO>>(okResult.Value);
        Assert.Single(returnedStudents);
        Assert.Null(returnedStudents[0].Status);
    }

    [Fact]
    public async Task GetStudentsWithAttendance_ShouldReturn403_WhenUnauthorized()
    {
        // Arrange
        _mockAttendanceService
            .Setup(s => s.GetStudentsWithAttendanceAsync(_testCourseId, _testDate, _testUserId))
            .ThrowsAsync(new UnauthorizedAccessException("You are not assigned to this course."));

        // Act
        var result = await _controller.GetStudentsWithAttendance(_testCourseId, _testDate);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(403, statusResult.StatusCode);
        var errorResponse = Assert.IsType<Dictionary<string, object>>(statusResult.Value);
        Assert.Contains("error", errorResponse.Keys);
        _mockAttendanceService.Verify(s => s.GetStudentsWithAttendanceAsync(_testCourseId, _testDate, _testUserId), Times.Once);
    }

    [Fact]
    public async Task GetStudentsWithAttendance_ShouldReturn500_WhenServiceThrowsException()
    {
        // Arrange
        _mockAttendanceService
            .Setup(s => s.GetStudentsWithAttendanceAsync(_testCourseId, _testDate, _testUserId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetStudentsWithAttendance(_testCourseId, _testDate);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region SaveAttendance Tests

    [Fact]
    public async Task SaveAttendance_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        var request = new SaveAttendanceRequestDTO
        {
            CourseId = _testCourseId,
            Date = _testDate,
            Records = new List<AttendanceRecordDTO>
            {
                new AttendanceRecordDTO
                {
                    StudentId = 1,
                    Status = "Present",
                    Note = "On time"
                },
                new AttendanceRecordDTO
                {
                    StudentId = 2,
                    Status = "Absent",
                    Note = "Excused"
                }
            }
        };

        _mockAttendanceService
            .Setup(s => s.SaveAttendanceAsync(It.IsAny<SaveAttendanceRequestDTO>(), _testUserId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.SaveAttendance(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Dictionary<string, object>>(okResult.Value);
        Assert.Contains("message", response.Keys);
        _mockAttendanceService.Verify(s => s.SaveAttendanceAsync(
            It.Is<SaveAttendanceRequestDTO>(r => r.CourseId == _testCourseId && r.Records.Count == 2),
            _testUserId), Times.Once);
    }

    [Fact]
    public async Task SaveAttendance_ShouldReturnOk_WhenAllStatusTypes()
    {
        // Arrange
        var request = new SaveAttendanceRequestDTO
        {
            CourseId = _testCourseId,
            Date = _testDate,
            Records = new List<AttendanceRecordDTO>
            {
                new AttendanceRecordDTO { StudentId = 1, Status = "Present", Note = "On time" },
                new AttendanceRecordDTO { StudentId = 2, Status = "Absent", Note = "Excused" },
                new AttendanceRecordDTO { StudentId = 3, Status = "Late", Note = "15 minutes late" }
            }
        };

        _mockAttendanceService
            .Setup(s => s.SaveAttendanceAsync(It.IsAny<SaveAttendanceRequestDTO>(), _testUserId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.SaveAttendance(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        _mockAttendanceService.Verify(s => s.SaveAttendanceAsync(
            It.Is<SaveAttendanceRequestDTO>(r => 
                r.Records.Any(rec => rec.Status == "Present") &&
                r.Records.Any(rec => rec.Status == "Absent") &&
                r.Records.Any(rec => rec.Status == "Late")),
            _testUserId), Times.Once);
    }

    [Fact]
    public async Task SaveAttendance_ShouldReturnOk_WhenNoteIsNull()
    {
        // Arrange
        var request = new SaveAttendanceRequestDTO
        {
            CourseId = _testCourseId,
            Date = _testDate,
            Records = new List<AttendanceRecordDTO>
            {
                new AttendanceRecordDTO
                {
                    StudentId = 1,
                    Status = "Present",
                    Note = null
                }
            }
        };

        _mockAttendanceService
            .Setup(s => s.SaveAttendanceAsync(It.IsAny<SaveAttendanceRequestDTO>(), _testUserId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.SaveAttendance(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        _mockAttendanceService.Verify(s => s.SaveAttendanceAsync(It.IsAny<SaveAttendanceRequestDTO>(), _testUserId), Times.Once);
    }

    [Fact]
    public async Task SaveAttendance_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var request = new SaveAttendanceRequestDTO
        {
            CourseId = _testCourseId,
            Date = _testDate,
            Records = new List<AttendanceRecordDTO>() // Empty records
        };

        _controller.ModelState.AddModelError("Records", "At least one attendance record is required.");

        // Act
        var result = await _controller.SaveAttendance(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        _mockAttendanceService.Verify(s => s.SaveAttendanceAsync(It.IsAny<SaveAttendanceRequestDTO>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SaveAttendance_ShouldReturnBadRequest_WhenInvalidStatus()
    {
        // Arrange
        var request = new SaveAttendanceRequestDTO
        {
            CourseId = _testCourseId,
            Date = _testDate,
            Records = new List<AttendanceRecordDTO>
            {
                new AttendanceRecordDTO
                {
                    StudentId = 1,
                    Status = "InvalidStatus", // Invalid status
                    Note = null
                }
            }
        };

        _controller.ModelState.AddModelError("Records[0].Status", "Status must be Present, Absent, or Late.");

        // Act
        var result = await _controller.SaveAttendance(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        _mockAttendanceService.Verify(s => s.SaveAttendanceAsync(It.IsAny<SaveAttendanceRequestDTO>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SaveAttendance_ShouldReturnBadRequest_WhenInvalidStudentIds()
    {
        // Arrange
        var request = new SaveAttendanceRequestDTO
        {
            CourseId = _testCourseId,
            Date = _testDate,
            Records = new List<AttendanceRecordDTO>
            {
                new AttendanceRecordDTO
                {
                    StudentId = 999, // Invalid student ID
                    Status = "Present",
                    Note = null
                }
            }
        };

        _mockAttendanceService
            .Setup(s => s.SaveAttendanceAsync(It.IsAny<SaveAttendanceRequestDTO>(), _testUserId))
            .ThrowsAsync(new ArgumentException("Invalid student IDs: 999. These students are not enrolled in the course."));

        // Act
        var result = await _controller.SaveAttendance(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<Dictionary<string, object>>(badRequestResult.Value);
        Assert.Contains("error", errorResponse.Keys);
        Assert.Contains("999", errorResponse["error"]?.ToString() ?? "");
        _mockAttendanceService.Verify(s => s.SaveAttendanceAsync(It.IsAny<SaveAttendanceRequestDTO>(), _testUserId), Times.Once);
    }

    [Fact]
    public async Task SaveAttendance_ShouldReturnBadRequest_WhenCourseIdIsEmpty()
    {
        // Arrange
        var request = new SaveAttendanceRequestDTO
        {
            CourseId = Guid.Empty, // Invalid course ID
            Date = _testDate,
            Records = new List<AttendanceRecordDTO>
            {
                new AttendanceRecordDTO { StudentId = 1, Status = "Present" }
            }
        };

        _controller.ModelState.AddModelError("CourseId", "CourseId is required.");

        // Act
        var result = await _controller.SaveAttendance(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        _mockAttendanceService.Verify(s => s.SaveAttendanceAsync(It.IsAny<SaveAttendanceRequestDTO>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SaveAttendance_ShouldReturn403_WhenUnauthorized()
    {
        // Arrange
        var request = new SaveAttendanceRequestDTO
        {
            CourseId = _testCourseId,
            Date = _testDate,
            Records = new List<AttendanceRecordDTO>
            {
                new AttendanceRecordDTO
                {
                    StudentId = 1,
                    Status = "Present",
                    Note = null
                }
            }
        };

        _mockAttendanceService
            .Setup(s => s.SaveAttendanceAsync(It.IsAny<SaveAttendanceRequestDTO>(), _testUserId))
            .ThrowsAsync(new UnauthorizedAccessException("You are not assigned to this course."));

        // Act
        var result = await _controller.SaveAttendance(request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(403, statusResult.StatusCode);
        var errorResponse = Assert.IsType<Dictionary<string, object>>(statusResult.Value);
        Assert.Contains("error", errorResponse.Keys);
        Assert.Equal("You are not assigned to this course.", errorResponse["error"]);
        _mockAttendanceService.Verify(s => s.SaveAttendanceAsync(It.IsAny<SaveAttendanceRequestDTO>(), _testUserId), Times.Once);
    }

    [Fact]
    public async Task SaveAttendance_ShouldReturn500_WhenServiceThrowsException()
    {
        // Arrange
        var request = new SaveAttendanceRequestDTO
        {
            CourseId = _testCourseId,
            Date = _testDate,
            Records = new List<AttendanceRecordDTO>
            {
                new AttendanceRecordDTO
                {
                    StudentId = 1,
                    Status = "Present",
                    Note = null
                }
            }
        };

        _mockAttendanceService
            .Setup(s => s.SaveAttendanceAsync(It.IsAny<SaveAttendanceRequestDTO>(), _testUserId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.SaveAttendance(request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetAttendanceStatistics Tests

    [Fact]
    public async Task GetAttendanceStatistics_ShouldReturnOk_WhenAuthorized()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);
        var expectedStats = new AttendanceStatisticsDTO
        {
            TotalRecords = 100,
            PresentCount = 70,
            AbsentCount = 20,
            LateCount = 10,
            PresentPercentage = 70.0,
            AbsentPercentage = 20.0,
            LatePercentage = 10.0
        };

        _mockAttendanceService
            .Setup(s => s.GetAttendanceStatisticsAsync(_testCourseId, startDate, endDate, _testUserId))
            .ReturnsAsync(expectedStats);

        // Act
        var result = await _controller.GetAttendanceStatistics(_testCourseId, startDate, endDate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStats = Assert.IsType<AttendanceStatisticsDTO>(okResult.Value);
        Assert.Equal(100, returnedStats.TotalRecords);
        Assert.Equal(70, returnedStats.PresentCount);
        Assert.Equal(20, returnedStats.AbsentCount);
        Assert.Equal(10, returnedStats.LateCount);
        Assert.Equal(70.0, returnedStats.PresentPercentage);
        _mockAttendanceService.Verify(s => s.GetAttendanceStatisticsAsync(_testCourseId, startDate, endDate, _testUserId), Times.Once);
    }

    [Fact]
    public async Task GetAttendanceStatistics_ShouldReturnOk_WhenNoRecords()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);
        var expectedStats = new AttendanceStatisticsDTO
        {
            TotalRecords = 0,
            PresentCount = 0,
            AbsentCount = 0,
            LateCount = 0,
            PresentPercentage = 0.0,
            AbsentPercentage = 0.0,
            LatePercentage = 0.0
        };

        _mockAttendanceService
            .Setup(s => s.GetAttendanceStatisticsAsync(_testCourseId, startDate, endDate, _testUserId))
            .ReturnsAsync(expectedStats);

        // Act
        var result = await _controller.GetAttendanceStatistics(_testCourseId, startDate, endDate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStats = Assert.IsType<AttendanceStatisticsDTO>(okResult.Value);
        Assert.Equal(0, returnedStats.TotalRecords);
    }

    [Fact]
    public async Task GetAttendanceStatistics_ShouldReturn403_WhenUnauthorized()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);

        _mockAttendanceService
            .Setup(s => s.GetAttendanceStatisticsAsync(_testCourseId, startDate, endDate, _testUserId))
            .ThrowsAsync(new UnauthorizedAccessException("You are not assigned to this course."));

        // Act
        var result = await _controller.GetAttendanceStatistics(_testCourseId, startDate, endDate);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(403, statusResult.StatusCode);
        var errorResponse = Assert.IsType<Dictionary<string, object>>(statusResult.Value);
        Assert.Contains("error", errorResponse.Keys);
        _mockAttendanceService.Verify(s => s.GetAttendanceStatisticsAsync(_testCourseId, startDate, endDate, _testUserId), Times.Once);
    }

    [Fact]
    public async Task GetAttendanceStatistics_ShouldReturnOk_WithCorrectPercentages()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);
        var expectedStats = new AttendanceStatisticsDTO
        {
            TotalRecords = 30,
            PresentCount = 20,
            AbsentCount = 7,
            LateCount = 3,
            PresentPercentage = 66.67,
            AbsentPercentage = 23.33,
            LatePercentage = 10.0
        };

        _mockAttendanceService
            .Setup(s => s.GetAttendanceStatisticsAsync(_testCourseId, startDate, endDate, _testUserId))
            .ReturnsAsync(expectedStats);

        // Act
        var result = await _controller.GetAttendanceStatistics(_testCourseId, startDate, endDate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStats = Assert.IsType<AttendanceStatisticsDTO>(okResult.Value);
        Assert.Equal(30, returnedStats.TotalRecords);
        Assert.Equal(66.67, returnedStats.PresentPercentage);
        Assert.Equal(23.33, returnedStats.AbsentPercentage);
        Assert.Equal(10.0, returnedStats.LatePercentage);
    }

    [Fact]
    public async Task GetAttendanceStatistics_ShouldReturn500_WhenServiceThrowsException()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);

        _mockAttendanceService
            .Setup(s => s.GetAttendanceStatisticsAsync(_testCourseId, startDate, endDate, _testUserId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAttendanceStatistics(_testCourseId, startDate, endDate);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region ExportAttendanceReport Tests

    [Fact]
    public async Task ExportAttendanceReport_ShouldReturnFile_WhenAuthorized()
    {
        // Arrange
        var reportBytes = System.Text.Encoding.UTF8.GetBytes("Index Number,First Name,Last Name,Status,Note\nA-001,John,Doe,Present,On time");
        
        _mockAttendanceService
            .Setup(s => s.ExportAttendanceReportAsync(_testCourseId, _testDate, _testUserId))
            .ReturnsAsync(reportBytes);

        // Act
        var result = await _controller.ExportAttendanceReport(_testCourseId, _testDate);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.Equal($"attendance_{_testCourseId}_{_testDate:yyyy-MM-dd}.csv", fileResult.FileDownloadName);
        Assert.Equal(reportBytes, fileResult.FileContents);
        _mockAttendanceService.Verify(s => s.ExportAttendanceReportAsync(_testCourseId, _testDate, _testUserId), Times.Once);
    }

    [Fact]
    public async Task ExportAttendanceReport_ShouldReturn403_WhenUnauthorized()
    {
        // Arrange
        _mockAttendanceService
            .Setup(s => s.ExportAttendanceReportAsync(_testCourseId, _testDate, _testUserId))
            .ThrowsAsync(new UnauthorizedAccessException("You are not assigned to this course."));

        // Act
        var result = await _controller.ExportAttendanceReport(_testCourseId, _testDate);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(403, statusResult.StatusCode);
        var errorResponse = Assert.IsType<Dictionary<string, object>>(statusResult.Value);
        Assert.Contains("error", errorResponse.Keys);
        _mockAttendanceService.Verify(s => s.ExportAttendanceReportAsync(_testCourseId, _testDate, _testUserId), Times.Once);
    }

    [Fact]
    public async Task ExportAttendanceReport_ShouldReturnFile_WithCorrectContent()
    {
        // Arrange
        var csvContent = "Index Number,First Name,Last Name,Status,Note\nA-001,John,Doe,Present,On time\nA-002,Jane,Smith,Absent,";
        var reportBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
        
        _mockAttendanceService
            .Setup(s => s.ExportAttendanceReportAsync(_testCourseId, _testDate, _testUserId))
            .ReturnsAsync(reportBytes);

        // Act
        var result = await _controller.ExportAttendanceReport(_testCourseId, _testDate);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.Equal(reportBytes.Length, fileResult.FileContents.Length);
        var content = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
        Assert.Contains("Index Number", content);
        Assert.Contains("A-001", content);
    }

    [Fact]
    public async Task ExportAttendanceReport_ShouldReturn500_WhenServiceThrowsException()
    {
        // Arrange
        _mockAttendanceService
            .Setup(s => s.ExportAttendanceReportAsync(_testCourseId, _testDate, _testUserId))
            .ThrowsAsync(new Exception("Export error"));

        // Act
        var result = await _controller.ExportAttendanceReport(_testCourseId, _testDate);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public async Task GetStudentsWithAttendance_ShouldReturn403_WhenUserIdClaimMissing()
    {
        // Arrange
        var controllerWithoutAuth = new AttendanceController(_mockAttendanceService.Object);
        controllerWithoutAuth.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity()) // No claims
            }
        };

        // Act
        var result = await controllerWithoutAuth.GetStudentsWithAttendance(_testCourseId, _testDate);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(403, statusResult.StatusCode);
    }

    [Fact]
    public async Task SaveAttendance_ShouldReturn403_WhenUserIdClaimMissing()
    {
        // Arrange
        var controllerWithoutAuth = new AttendanceController(_mockAttendanceService.Object);
        controllerWithoutAuth.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity()) // No claims
            }
        };

        var request = new SaveAttendanceRequestDTO
        {
            CourseId = _testCourseId,
            Date = _testDate,
            Records = new List<AttendanceRecordDTO>
            {
                new AttendanceRecordDTO { StudentId = 1, Status = "Present" }
            }
        };

        // Act
        var result = await controllerWithoutAuth.SaveAttendance(request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(403, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetAttendanceStatistics_ShouldReturn403_WhenUserIdClaimMissing()
    {
        // Arrange
        var controllerWithoutAuth = new AttendanceController(_mockAttendanceService.Object);
        controllerWithoutAuth.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity()) // No claims
            }
        };

        // Act
        var result = await controllerWithoutAuth.GetAttendanceStatistics(_testCourseId, DateTime.Now, DateTime.Now);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(403, statusResult.StatusCode);
    }

    [Fact]
    public async Task ExportAttendanceReport_ShouldReturn403_WhenUserIdClaimMissing()
    {
        // Arrange
        var controllerWithoutAuth = new AttendanceController(_mockAttendanceService.Object);
        controllerWithoutAuth.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity()) // No claims
            }
        };

        // Act
        var result = await controllerWithoutAuth.ExportAttendanceReport(_testCourseId, _testDate);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(403, statusResult.StatusCode);
    }

    #endregion
}

