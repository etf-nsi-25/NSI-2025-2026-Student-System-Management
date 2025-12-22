using Analytics.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Faculty.Core.Interfaces;
using Faculty.Core.Services;


namespace Analytics.API.Controllers
{
    [ApiController]
    [Route("api/Analytics")]
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
        public async Task<IActionResult> GetStudentStats()
        {
            var facultyId = _tenantService.GetCurrentFacultyId().ToString();

            var userId = "user123"; // hardcoded 

            try
            {
                var stats = await _analyticsService.GetStudentStatsAsync(userId, facultyId);

                if (stats == null) return NotFound("Data not found.");

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}