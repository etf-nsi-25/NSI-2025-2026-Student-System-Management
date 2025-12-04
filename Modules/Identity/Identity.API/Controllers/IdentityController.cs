using Identity.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Identity.Core.DTO;
using Identity.Core.Enums;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class IdentityController : ControllerBase
    {
        private readonly IUserService _userService;

        public IdentityController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = await _userService.CreateUserAsync(
                    request.Username,
                    request.Password,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.FacultyId,
                    request.IndexNumber,
                    request.Role
                );

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { userId = userId },
                    new UserResponse
                    {
                        Id = userId,
                        Username = request.Username,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        IndexNumber = request.IndexNumber,
                        Role = request.Role,
                        FacultyId = request.FacultyId,
                        Status = UserStatus.Active
                    }
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (ArgumentException ex) when (ex.ParamName == "username")
            {
                return Conflict(new { Error = "Username already exists." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An unexpected error occurred. Please try again." });
            }
        }

        [HttpGet("{userId:guid}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterRequest filter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usersList = await _userService.GetAllUsersAsync(filter);
            return Ok(usersList);
        }

        [HttpPut("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userService.UpdateUserAsync(userId, request);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch("{userId:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeactivateUser(Guid userId)
        {
            try
            {

                var result = await _userService.DeactivateUserAsync(userId);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                var deleted = await _userService.DeleteUserAsync(userId);

                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}