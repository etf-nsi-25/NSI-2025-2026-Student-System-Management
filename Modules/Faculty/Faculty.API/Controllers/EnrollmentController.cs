using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using global::Faculty.Application.DTOs;
using global::Faculty.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace Faculty.API.Controllers
{
    [ApiController]
    [Route("api/faculty/enrollments")]
    //[Authorize] // JWT required
    [AllowAnonymous]  //FOR TEMPORARY TESTING ONLY SINCE LOGIN IS NOT WORKING

    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // GET /api/faculty/enrollments
        [HttpGet]
        public async Task<ActionResult<List<EnrollmentListItemDto>>> GetMyEnrollments()
        {
            var result = await _enrollmentService.GetMyEnrollmentsAsync();
            return Ok(result);
        }

        // POST /api/faculty/enrollments
        [HttpPost]
        public async Task<ActionResult<CreateEnrollmentResponseDto>> CreateEnrollment(
            [FromBody] CreateEnrollmentRequestDto dto)
        {
            try
            {
                var result = await _enrollmentService.CreateEnrollmentAsync(dto);
                return Created("", result); // 201
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
