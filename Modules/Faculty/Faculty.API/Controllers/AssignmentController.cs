using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faculty.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssignmentController(IAssignmentService assignmentService) : ControllerBase
    {
        readonly private IAssignmentService _assignmentService = assignmentService;

        /// <summary>
        /// Gets the current user ID from JWT claims.
        /// </summary>
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("UserId claim not found in token.");
            }
            return new Guid(userIdClaim.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAssigment(CreateAssignmentDTO assignmentDTO)
        {
           var userId = GetCurrentUserId();
           var result = await _assignmentService.CreateAssignment(assignmentDTO, userId);
           
            if(result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            else
            {
                return Created();
            }
        }

        [HttpDelete("{assignmentID}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAssigment([FromRoute] int assignmentID)
        {
            var result = await _assignmentService.DeleteAssignment(assignmentID);
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            else if (result.StatusCode == StatusCodes.Status200OK)
            {
                return Ok(result);
            }

            return Problem();
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<PaginatedDTO<AssignmentDTO>> GetAssigments(
            [FromQuery] string? query,
            [FromQuery] int pageSize = 4,
            [FromQuery] int pageNumber = 1)
        {
            var userId = GetCurrentUserId();
            return await _assignmentService.GetAssignmentsByUserId(userId, query, pageSize, pageNumber);
        }

        [HttpPut("{assignmentID}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAssigment([FromRoute] int assignmentID, [FromQuery] CreateAssignmentDTO assignmentDTO)
        {
            var userId = GetCurrentUserId();
            var result = await _assignmentService.UpdateAssignment(assignmentID, assignmentDTO, userId);

            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            else
            {
                return Created();
            }
        }
    }
}
