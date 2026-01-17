using Analytics.Application.Interfaces;
using Analytics.Core.Constants;
using Analytics.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace Analytics.API.Controllers;

[ApiController]
[Route("api/stats")]
public class AnalyticsController : ControllerBase
{
    private readonly IStatsService _statsService;
    private readonly ITeacherAnalyticsService _teacherAnalyticsService;

    public AnalyticsController(IStatsService statsService, ITeacherAnalyticsService teacherAnalyticsService)
    {
        _statsService = statsService;
        _teacherAnalyticsService = teacherAnalyticsService;
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

    
    [Authorize] 
    [HttpGet("teacher/filter-data")] 
    public async Task<IActionResult> GetTeacherFilterData()
    {
        
        var email = User.FindFirstValue(ClaimTypes.Email); 
        
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized("Email nije pronađen u tokenu.");
        }

        
        var data = await _teacherAnalyticsService.GetTeacherFilterDataByEmailAsync(email);

        if (data == null)
        {
            return NotFound($"Profesor sa emailom {email} nije pronađen u bazi.");
        }

        return Ok(data);
    }

}

