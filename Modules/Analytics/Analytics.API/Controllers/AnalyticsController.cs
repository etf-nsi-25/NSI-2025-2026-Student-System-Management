using Analytics.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Faculty.Infrastructure.Http;
using System.Security.Claims;

namespace Analytics.API.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IStudentAnalyticsService _analyticsService;
    private readonly ITenantService _tenantService;

    public AnalyticsController(IStudentAnalyticsService analyticsService, ITenantService tenantService)
    {
        _analyticsService = analyticsService;
        _tenantService = tenantService;
    }

    [HttpGet("student-performance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentStats()
    {
        try
        {
            var facultyId = _tenantService.GetCurrentFacultyId().ToString();

            var userId = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User identity could not be verified from the provided token.");
            }

            var stats = await _analyticsService.GetStudentStatsAsync(userId, facultyId);

            return stats is null
                ? NotFound($"Statistics not found for student {userId} at faculty {facultyId}.")
                : Ok(stats);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An internal error occurred: {ex.Message}");
        }
    }
}