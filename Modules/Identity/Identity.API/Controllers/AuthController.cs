// Identity.API/Controllers/AuthController.cs
using Identity.API.DTO.Auth;
using Identity.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user using email and password.
    /// Returns a JWT access token and sets an HTTP-only refresh token cookie if credentials are valid.
    /// </summary>
    /// <remarks>
    /// ## Example request:
    /// 
    ///     {
    ///         "email": "student@example.com",
    ///         "password": "Password123!"
    ///     }
    ///
    /// ## Example successful response (200):
    ///
    ///     {
    ///         "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    ///         "tokenType": "Bearer"
    ///     }
    ///
    /// The refresh token is NOT included in the JSON body.
    /// It is sent as an HTTP-only secure cookie named <c>refreshToken</c>.
    ///
    /// ## Example invalid credentials (401):
    ///
    ///     {
    ///         "message": "Invalid email or password"
    ///     }
    ///
    /// ## Example validation error (400):
    ///
    ///     {
    ///         "email": [ "The Email field is required." ]
    ///     }
    ///
    /// ## Example internal error (500):
    ///
    ///     {
    ///         "message": "An error occurred during login"
    ///     }
    /// </remarks>
    /// <response code="200">User successfully authenticated. Returns access token.</response>
    /// <response code="400">Invalid request body or missing required fields.</response>
    /// <response code="401">Invalid email or password.</response>
    /// <response code="500">Unexpected error during authentication.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "unknown";

            var result = await _authService.AuthenticateAsync(
                request.Email,
                request.Password,
                ipAddress,
                userAgent);

            // Set HTTP-only cookie for refresh token
            HttpContext.Response.Cookies.Append(
                "refreshToken",
                result.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = result.ExpiresAt
                });

            var response = new LoginResponseDto
            {
                AccessToken = result.AccessToken,
                TokenType = "Bearer"
            };

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Login failed for email: {Email}", request.Email);
            return Unauthorized(new { message = "Invalid email or password" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }


    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> RefreshToken()
    {
        try
        {
            if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "unknown";

            var result = await _authService.RefreshAuthenticationAsync(
                refreshToken,
                ipAddress,
                userAgent);

            // Set HTTP-only cookie for new refresh token
            HttpContext.Response.Cookies.Append(
                "refreshToken",
                result.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = result.ExpiresAt
                });

            // Map domain model to DTO
            var response = new LoginResponseDto
            {
                AccessToken = result.AccessToken,
                TokenType = "Bearer"
            };

            return Ok(response);

        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Token refresh failed");
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new { message = "An error occurred during token refresh" });
        }
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            await _authService.RevokeAuthenticationAsync(refreshToken);

            // Clear the refresh token cookie
            HttpContext.Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Successfully logged out" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { message = "An error occurred during logout" });
        }
    }
}