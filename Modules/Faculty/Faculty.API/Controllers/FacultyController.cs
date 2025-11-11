using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Faculty.API.Models; // Add this using statement

namespace Faculty.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protect all endpoints in this controller
    public class FacultyController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("Hello from Faculty API!");

        [HttpPost]
        [Authorize(Roles = "Superadmin")]
        public IActionResult CreateFaculty([FromBody] CreateFacultyRequest request)
        {
            // Placeholder for faculty creation logic
            return Ok($"Faculty '{request.FacultyName}' created by Superadmin.");
        }

        [HttpGet("throw-exception")]
        [Authorize(Roles = "Superadmin")]
        public IActionResult ThrowException()
        {
            throw new InvalidOperationException("This is a test exception to trigger HTTP 500 error handling.");
        }
    }
}
