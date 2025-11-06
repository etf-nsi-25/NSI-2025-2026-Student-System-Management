// Identity.API/Controllers/IdentityController.cs
using Identity.Application.DTOs.Auth;
using Identity.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/identity")]
[Produces("application/json")]
public class IdentityController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<IdentityController> _logger;

    public IdentityController(IAuthService authService, ILogger<IdentityController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Returns the RSA public key for JWT token verification by other microservices
    /// </summary>
    /// <returns>RSA public key in Base64 format</returns>
    /// <response code="200">Public key retrieved successfully</response>
    /// 
    [HttpGet("public-key")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PublicKeyResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PublicKeyResponseDto>> GetPublicKey()
    {
        try
        {
            var result = await _authService.GetPublicKeyInfoAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving public key");
            return StatusCode(500, new { message = "An error occurred retrieving the public key" });
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}