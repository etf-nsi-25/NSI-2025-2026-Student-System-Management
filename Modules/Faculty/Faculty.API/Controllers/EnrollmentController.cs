using System.Security.Claims;
using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faculty.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Student")]
    [Route("api/faculty/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _service;

        public EnrollmentsController(IEnrollmentService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateEnrollmentRequestDto request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserIdOrThrow();

            var result = await _service.CreateEnrollmentAsync(
                request.CourseId,
                userId,
                cancellationToken);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyEnrollments(
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserIdOrThrow();

            var result = await _service.GetMyEnrollmentsAsync(
                userId,
                cancellationToken);

            return Ok(result);
        }

        private string GetCurrentUserIdOrThrow()
        {
            var userId =
                User.FindFirstValue("userId")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("Missing user identifier claim.");

            return userId;
        }
    }
}
