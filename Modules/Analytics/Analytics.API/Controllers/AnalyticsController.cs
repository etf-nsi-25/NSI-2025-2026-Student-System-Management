using Analytics.Application.Interfaces;
using Analytics.Core.Constants;
using Analytics.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Route("api/stats")]
public class AnalyticsController : ControllerBase
{
    private readonly IStatsService _statsService;
    public AnalyticsController(IStatsService statsService)
    {
        _statsService = statsService;
    }

    [HttpGet("total-students")]
    public async Task<IActionResult> GetTotalStudents()
    {
        var data = await _statsService.GetOrUpdateStatAsync(
            MetricKey.CountStudents,
            Scope.University,
            Guid.Empty
        );

        return Ok(data);
    }

}

