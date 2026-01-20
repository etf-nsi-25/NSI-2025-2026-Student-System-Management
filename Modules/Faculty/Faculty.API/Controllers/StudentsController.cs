using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faculty.API.Controllers;

[ApiController]
[Route("api/faculty/students/me")]
[Authorize(Roles = "Student")]
public class StudentsController : ControllerBase
{
    private readonly IStudentAnalyticsService _analyticsService;

    public StudentsController(IStudentAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
    }

    private string GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId");
        if (userIdClaim == null)
        {
            throw new UnauthorizedAccessException("UserId claim not found in token.");
        }
        return userIdClaim.Value;
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(StudentSummaryDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _analyticsService.GetStudentSummaryAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while loading student summary.", details = ex.Message });
        }
    }

    [HttpGet("schedule/weekly")]
    [ProducesResponseType(typeof(WeeklyScheduleDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWeeklySchedule(
        [FromQuery] DateTime startDate,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _analyticsService.GetWeeklyScheduleAsync(userId, startDate, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while loading weekly schedule.", details = ex.Message });
        }
    }

    [HttpGet("calendar/month")]
    [ProducesResponseType(typeof(MonthlyCalendarDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMonthlyCalendar(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        try
        {
            if (year < 2000 || year > 2100)
            {
                return BadRequest(new { error = "Year must be between 2000 and 2100." });
            }

            if (month < 1 || month > 12)
            {
                return BadRequest(new { error = "Month must be between 1 and 12." });
            }

            var userId = GetCurrentUserId();
            var result = await _analyticsService.GetMonthlyCalendarAsync(userId, year, month, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while loading monthly calendar.", details = ex.Message });
        }
    }

    [HttpGet("attendance")]
    [ProducesResponseType(typeof(StudentAttendanceStatsDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAttendance(
        [FromQuery] Guid courseId,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _analyticsService.GetAttendanceStatsAsync(userId, courseId, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while loading attendance statistics.", details = ex.Message });
        }
    }

    [HttpGet("subjects/progress")]
    [ProducesResponseType(typeof(SubjectProgressDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubjectProgress(
        [FromQuery] Guid? semesterId,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _analyticsService.GetSubjectProgressAsync(userId, semesterId, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while loading subject progress.", details = ex.Message });
        }
    }
}

