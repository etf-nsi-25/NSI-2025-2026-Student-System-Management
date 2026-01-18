using Analytics.Application.Interfaces;
using Analytics.Core.Constants;
using Analytics.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Analytics.Application.DTO;


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

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetCourseStats([FromRoute] Guid courseId)
    {
        var data = await _statsService.GetOrUpdateStatAsync(
            MetricKey.GradeDistrib,
            Scope.Course,
            courseId
        );

        if (data == null)
            return NotFound();

        var dto = new CourseStatsDto { CourseId = courseId };

        int total = 0;
        int passed = 0;

        foreach (var prop in data)
        {
            var key = prop.Key;
            var node = prop.Value;
            int val = 0;
            try
            {
                val = node != null ? node.GetValue<int>() : 0;
            }
            catch
            {
                continue;
            }

            dto.Distribution[key] = val;
            total += val;

            if (int.TryParse(key, out var g) && g >= 5)
            {
                passed += val;
            }
        }

        dto.TotalCount = total;
        dto.PassedCount = passed;

        return Ok(dto);
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

