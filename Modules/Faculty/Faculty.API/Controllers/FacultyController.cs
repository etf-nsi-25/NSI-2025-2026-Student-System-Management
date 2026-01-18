using Faculty.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Faculty.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;   


namespace Faculty.API.Controllers
{
    [ApiController]
    [Route("api/faculty/courses")]
    [Authorize]
    public class FacultyController : ControllerBase
    {
        private readonly ICourseService _service;
        private readonly IUpcomingActivityService _upcomingActivityService;

        public FacultyController(ICourseService service, IUpcomingActivityService upcomingActivityService)
        {
            _service = service;
            _upcomingActivityService = upcomingActivityService;
        }

        private string GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value))
            {
                throw new UnauthorizedAccessException("UserId claim not found in token.");
            }

            return userIdClaim.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("assigned")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetAssignedToTeacher()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _service.GetProfessorCoursesAsync(userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(await _service.AddAsync(dto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CourseDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool ok = await _service.DeleteAsync(id);
            return ok ? Ok(new { success = true }) : NotFound();
        }

        [HttpGet("{courseId}/teacher")]
        [Authorize]
        public async Task<IActionResult> GetTeacherForCourse(Guid courseId)
        {
            var teacher = await _service.GetTeacherForCourseAsync(courseId);
            return teacher == null ? NotFound() : Ok(teacher);
        }


        [HttpGet("upcoming-activities")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetUpcomingActivities()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _upcomingActivityService.GetUpcomingActivitiesAsync(userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}



