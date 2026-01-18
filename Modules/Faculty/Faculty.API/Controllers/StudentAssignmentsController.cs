using Faculty.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Faculty.Core.Enums;

namespace Faculty.API.Controllers;

[ApiController]
[Route("api/faculty/my-assignments")]
[Authorize(Roles = "Student")]
public class StudentAssignmentsController : ControllerBase
{
    private readonly IAssignmentService _service;

    public StudentAssignmentsController(IAssignmentService service) => _service = service;
    public AssignmentStatus Status { get; set; }

    [HttpGet("courses/{courseId}")]
    public async Task<IActionResult> GetAssignments(Guid courseId)
    {
        try
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _service.GetStudentAssignmentsForCourseAsync(courseId, userId);
            return Ok(result);
        }
        catch (Faculty.Application.Exceptions.FacultyApplicationException ex)
        {
            return StatusCode((int)ex.StatusCode, new { error = ex.Message });
        }
    }
}