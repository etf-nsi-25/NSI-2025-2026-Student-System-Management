using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Security.Claims;
using Analytics.Application.Services;
using Analytics.Application.Handlers;
using Analytics.Application.Queries;

using Microsoft.AspNetCore.Authorization;

namespace Analytics.API.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("student-performance")]
    [Authorize] 
    public async Task<IActionResult> GetMyAcademicAnalytics()
    {
        var userId = User.FindFirst("userId")?.Value;

        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _mediator.Send(new GetStudentAnalyticsQuery(userId));
        return Ok(result);
    }
}