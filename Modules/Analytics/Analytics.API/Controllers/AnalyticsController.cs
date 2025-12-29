using Analytics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Analytics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
		private readonly IAcademicPerformanceService _service;

		public AnalyticsController(IAcademicPerformanceService service)
		{
			_service = service;
		}

		[HttpGet]
        public IActionResult Get() => Ok("Hello from Analytics API!");

		[HttpGet("academic-performance")]
		[Authorize]
		public async Task<IActionResult> GetAcademicPerformance(CancellationToken ct)
		{
			var userId = User.FindFirstValue("userId");
			var facultyIdStr = User.FindFirstValue("tenantId");

			if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(facultyIdStr))
				return Unauthorized();

			if (!Guid.TryParse(facultyIdStr, out var facultyId))
				return Unauthorized();

			var dto = await _service.GetAsync(userId, facultyId, ct);
			return Ok(dto);
		}


	}
}
