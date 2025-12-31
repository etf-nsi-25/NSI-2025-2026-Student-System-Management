using System.Security.Claims;
using Faculty.Application.DTOs;
using Faculty.Application.Exceptions;
using Faculty.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faculty.API.Controllers;

[ApiController]
[Authorize(Roles = "Student")]
[Route("/api/faculty/enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;

    public EnrollmentsController(IEnrollmentService service)
    {
        _service = service;
    }

    // post /api/faculty/enrollments
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEnrollmentRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!TryResolveUserId(out var userId, out var errorResult))
            return errorResult!;

        try
        {
            var result = await _service.CreateEnrollmentAsync(request.CourseId, userId!, cancellationToken);
            return Ok(result);
        }
        catch (FacultyApplicationException ex)
        {
            return StatusCode((int)ex.StatusCode, new { error = ex.Message });
        }
    }

    // get /api/faculty/enrollments/debug-claims
    // privremeno za debug - poslije obriši
    [HttpGet("debug-claims")]
    [Authorize]
    public IActionResult DebugClaims()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }

    private bool TryResolveUserId(out string? userId, out IActionResult? errorResult)
    {
        // try multiple common claim types
        userId =
            User.FindFirst("userId")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            errorResult = Unauthorized(new { error = "Missing user identifier claim." });
            return false;
        }

        errorResult = null;
        return true;
    }
}
