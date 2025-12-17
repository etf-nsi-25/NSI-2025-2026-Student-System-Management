using Faculty.Application.DTOs;
using Faculty.Application.Exceptions;
using Faculty.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faculty.API.Controllers;
[ApiController]
[Authorize(Roles = "Student")]
[Route("api/faculty/student-exams")]
public class StudentExamsController : ControllerBase
{
    private readonly IStudentExamRegistrationService _service;

    public StudentExamsController(IStudentExamRegistrationService service)
    {
        _service = service;
    }

    [HttpPost("registrations")]
    public async Task<IActionResult> Register(
        [FromBody] ExamRegistrationRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryResolveIdentity(out var userId, out var facultyId, out var errorResult))
        {
            return errorResult!;
        }

        try
        {
            var registration = await _service.RegisterAsync(request.ExamId, userId!, facultyId, cancellationToken);
            return CreatedAtAction(nameof(GetRegistrations), null, registration);
        }
        catch (FacultyApplicationException ex)
        {
            return StatusCode((int)ex.StatusCode, new { error = ex.Message });
        }
    }
    
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableExams(CancellationToken cancellationToken)
    {
        if (!TryResolveIdentity(out var userId, out var facultyId, out var errorResult))
        {
            return errorResult!;
        }

        try
        {
            var exams = await _service.GetAvailableExamsAsync(userId!, facultyId, cancellationToken);
            return Ok(exams);
        }
        catch (FacultyApplicationException ex)
        {
            return StatusCode((int)ex.StatusCode, new { error = ex.Message });
        }
    }
    
    [HttpGet("registrations")]
    public async Task<IActionResult> GetRegistrations(CancellationToken cancellationToken)
    {
        if (!TryResolveIdentity(out var userId, out var facultyId, out var errorResult))
        {
            return errorResult!;
        }

        try
        {
            var registrations = await _service.GetRegistrationsAsync(userId!, facultyId, cancellationToken);
            return Ok(registrations);
        }
        catch (FacultyApplicationException ex)
        {
            return StatusCode((int)ex.StatusCode, new { error = ex.Message });
        }
    }

    private bool TryResolveIdentity(out string? userId, out Guid facultyId, out IActionResult? errorResult)
    {
        userId = User.FindFirst("userId")?.Value;
        var facultyClaim = User.FindFirst("tenantId")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            errorResult = Unauthorized(new { error = "Missing user identifier claim." });
            facultyId = Guid.Empty;
            return false;
        }

        if (!Guid.TryParse(facultyClaim, out facultyId))
        {
            errorResult = Unauthorized(new { error = "Missing faculty identifier claim." });
            return false;
        }

        errorResult = null;
        return true;
    }
}
