using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Faculty.API.Controllers;

/// <summary>
/// Controller for managing student attendance.
/// </summary>
[ApiController]
[Route("api/faculty/attendance")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService ?? throw new ArgumentNullException(nameof(attendanceService));
    }

    /// <summary>
    /// Gets the current user ID from JWT claims.
    /// </summary>
    private string GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId");
        if (userIdClaim == null)
        {
            throw new UnauthorizedAccessException("UserId claim not found in token.");
        }
        return userIdClaim.Value;
    }

    /// <summary>
    /// Load students with attendance for a selected course and date.
    /// GET /api/faculty/attendance/courses/{courseId}/date/{date}
    /// </summary>
    [HttpGet("courses/{courseId}/date/{date}")]
    [ProducesResponseType(typeof(List<StudentAttendanceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentsWithAttendance(Guid courseId, DateTime date)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _attendanceService.GetStudentsWithAttendanceAsync(courseId, date, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new Dictionary<string, object> { { "error", ex.Message } });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while loading students." },
                { "details", ex.Message }
            });
        }
    }

    /// <summary>
    /// Save attendance records for a course and date.
    /// POST /api/faculty/attendance
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SaveAttendance([FromBody] SaveAttendanceRequestDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetCurrentUserId();
            await _attendanceService.SaveAttendanceAsync(request, userId);
            return Ok(new Dictionary<string, object>
            {
                { "message", "Attendance saved successfully." }
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new Dictionary<string, object> { { "error", ex.Message } });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Dictionary<string, object> { { "error", ex.Message } });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while saving attendance." },
                { "details", ex.Message }
            });
        }
    }

    /// <summary>
    /// Get attendance statistics for a course and time period.
    /// GET /api/faculty/attendance/courses/{courseId}/statistics?startDate={startDate}&endDate={endDate}
    /// </summary>
    [HttpGet("courses/{courseId}/statistics")]
    [ProducesResponseType(typeof(AttendanceStatisticsDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAttendanceStatistics(
        Guid courseId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _attendanceService.GetAttendanceStatisticsAsync(courseId, startDate, endDate, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new Dictionary<string, object> { { "error", ex.Message } });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while loading statistics." },
                { "details", ex.Message }
            });
        }
    }

    /// <summary>
    /// Export attendance report for a course and date.
    /// GET /api/faculty/attendance/courses/{courseId}/date/{date}/export
    /// </summary>
    [HttpGet("courses/{courseId}/date/{date}/export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ExportAttendanceReport(Guid courseId, DateTime date)
    {
        try
        {
            var userId = GetCurrentUserId();
            var reportBytes = await _attendanceService.ExportAttendanceReportAsync(courseId, date, userId);
            
            var fileName = $"attendance_{courseId}_{date:yyyy-MM-dd}.csv";
            return File(reportBytes, "text/csv", fileName);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new Dictionary<string, object> { { "error", ex.Message } });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while exporting report." },
                { "details", ex.Message }
            });
        }
    }
}

