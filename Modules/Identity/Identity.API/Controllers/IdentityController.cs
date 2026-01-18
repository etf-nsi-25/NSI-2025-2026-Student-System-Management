using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Identity.Core.DTO;
using Identity.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requesterRole = GetCurrentUserRole();

            try
            {
                var userId = await _userService.CreateUserAsync(
                    request.Username,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.FacultyId,
                    request.IndexNumber,
                    request.Role,
                    requesterRole
                );

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { userId = userId },
                    new UserResponse
                    {
                        Id = userId,
                        Username = request.Username,
                        Email = request.Email,
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

        [HttpGet("me")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdStr = User.FindFirstValue("userId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserByIdAsync(userIdStr);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("me/change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeCurrentPassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirstValue("userId");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var result = await _userService.ChangePasswordAsync(userId, request.NewPassword);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(string userId)
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

        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var requesterRole = GetCurrentUserRole();
                var result = await _userService.UpdateUserAsync(userId, request, requesterRole);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                 return Forbid();
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

        [HttpPatch("{userId}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeactivateUser(string userId)
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

        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var requesterRole = GetCurrentUserRole();
                var deleted = await _userService.DeleteUserAsync(userId, requesterRole);

                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                 return Forbid();
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

        [HttpPost("{userId}/change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword(string userId, [FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (!CanModifyUser(userId))
                {
                    return Forbid();
                }

                var result = await _userService.ChangePasswordAsync(userId, request.NewPassword);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        private bool IsAdmin()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            return userRole == UserRole.Admin.ToString() || userRole == UserRole.Superadmin.ToString();
        }

        private UserRole? GetCurrentUserRole()
        {
            var roleStr = User.FindFirstValue(ClaimTypes.Role);
            if (!string.IsNullOrEmpty(roleStr) && Enum.TryParse<UserRole>(roleStr, out var role))
            {
                return role;
            }
            return null;
        }

        private bool IsSelf(string userId)
        {
            var currentUserId = User.FindFirstValue("userId");
            return currentUserId == userId.ToString();
        }

        private bool CanModifyUser(string userId)
        {
            return IsAdmin() || IsSelf(userId);
        }
    }
}