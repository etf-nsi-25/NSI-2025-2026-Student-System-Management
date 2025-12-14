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

        public FacultyController(ICourseService service)
        {
            _service = service;
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
        public async Task<IActionResult> GetAssignedToTeacher()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _service.GetByTeacherAsync(userId);
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
    }

    [ApiController]
    [Route("api/faculty/requests")] 

    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StudentRequestDto>), 200)]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _requestService.GetAllRequestsAsync();
            return Ok(requests);
        }

        [HttpPost("{requestId}/confirm")]
        [ProducesResponseType(typeof(StudentRequestDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateConfirmation(
            Guid requestId, 
            [FromBody] CreateConfirmationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedRequest = await _requestService.ProcessRequestAsync(requestId, request);
                
                return Ok(updatedRequest); 
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Request with the ID {requestId} is not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message }); 
            }
        }
    }

    }



