using Identity.API.DTO;
using Identity.Application.Interfaces;
using Identity.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/auth")]
public class TwoFactorAuthController : ControllerBase
{
    private readonly ITwoFactorAuthService _svc;
    private readonly ITwoFactorLoginSessionStore _twoFactorLoginSessionStore;
    private readonly IAuthService _authService;

    public TwoFactorAuthController(
        ITwoFactorAuthService svc,
        ITwoFactorLoginSessionStore twoFactorLoginSessionStore,
        IAuthService authService)
    {
        _svc = svc;
        _twoFactorLoginSessionStore = twoFactorLoginSessionStore;
        _authService = authService;
    }

    /// <summary>
    /// Starts TOTP-based two-factor authentication (2FA) setup for the current user.
    /// </summary>
    /// <remarks>
    /// Generates a new TOTP secret and returns data required to configure an
    /// authenticator application (QR code, manual entry key, otpauth URI).
    ///
    /// Example successful response:
    ///
    ///     {
    ///       "success": true,
    ///       "message": "Two-factor authentication setup initialized.",
    ///       "manualEntryKey": "JBSWY3DPEHPK3PXP",
    ///       "qrCodeBase64": "data:image/png;base64,iVBORw0KGgo...",
    ///       "otpAuthUri": "otpauth://totp/StudentSystem:user@example.com?secret=JBSWY3DPEHPK3PXP&amp;issuer=StudentSystem",
    ///       "error": "None"
    ///     }
    ///
    /// Example error response:
    ///
    ///     {
    ///       "success": false,
    ///       "message": "User not found.",
    ///       "manualEntryKey": null,
    ///       "qrCodeBase64": null,
    ///       "otpAuthUri": null,
    ///       "error": "UserNotFound"
    ///     }
    ///
    /// Security notes:
    /// - The TOTP secret is stored only in encrypted form.
    /// - The secret and recovery codes are never logged.
    /// </remarks>
    /// <response code="200">
    /// 2FA setup initialized. Check the response body for success and error fields.
    /// </response>
    [HttpPost("enable-2fa")]
    [Authorize]
    public async Task<IActionResult> Enable()
    {
        var userId = User.FindFirstValue("userId");
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new { message = "Missing userId claim." });
        }

        var res = await _svc.EnableTwoFactorAsync(userId);
        return Ok(res);
    }

    /// <summary>
    /// Confirms 2FA setup by validating the TOTP code entered by the user.
    /// </summary>
    /// <remarks>
    /// The user enters the TOTP code from the authenticator app to confirm 2FA setup.
    ///
    /// Example request:
    ///
    ///     {
    ///       "code": "123456"
    ///     }
    ///
    /// Example successful response:
    ///
    ///     {
    ///       "success": true,
    ///       "message": "Two-factor authentication has been enabled.",
    ///       "recoveryCodes": [
    ///         "39db-fa22-9c43",
    ///         "f832-bc18-23be",
    ///         "11ca-9b12-a3f1"
    ///       ],
    ///       "error": "None"
    ///     }
    ///
    /// Example invalid code:
    ///
    ///     {
    ///       "success": false,
    ///       "message": "The provided TOTP code is invalid.",
    ///       "recoveryCodes": null,
    ///       "error": "InvalidCode"
    ///     }
    ///
    /// Example rate limited:
    ///
    ///     {
    ///       "success": false,
    ///       "message": "Too many failed attempts. Please wait before trying again.",
    ///       "recoveryCodes": null,
    ///       "error": "RateLimited"
    ///     }
    ///
    /// Possible values of <c>error</c>:
    /// - <c>None</c>
    /// - <c>InvalidCode</c>
    /// - <c>RateLimited</c>
    /// - <c>UserNotFound</c>
    /// - <c>NotInitialized</c>
    /// </remarks>
    /// <param name="dto">Payload containing the TOTP code for confirming 2FA setup.</param>
    /// <response code="200">2FA successfully confirmed and enabled.</response>
    /// <response code="400">Invalid request or user state (e.g. 2FA not initialized).</response>
    /// <response code="401">Invalid TOTP code.</response>
    /// <response code="404">User not found.</response>
    /// <response code="429">Too many failed attempts (rate limited).</response>
    [HttpPost("verify-2fa-setup")]
    [Authorize]
    public async Task<IActionResult> VerifySetup([FromBody] TwoFAConfirmRequest dto)
    {
        var userId = User.FindFirstValue("userId");
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new { message = "Missing userId claim." });
        }

        var res = await _svc.VerifySetupAsync(userId, dto.Code);

        if (!res.Success)
            return BadRequest(res);

        return Ok(res);
    }

    /// <summary>
    /// Verifies a TOTP code during login for a user with 2FA enabled.
    /// </summary>
    /// <remarks>
    /// This endpoint is called after the primary login step (username + password)
    /// when the backend indicates that 2FA is required.
    ///
    /// Example request:
    ///
    ///     {
    ///       "code": "123456"
    ///     }
    ///
    /// Example successful response:
    ///
    ///     {
    ///       "success": true,
    ///       "message": "Two-factor authentication verified.",
    ///       "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    ///       "refreshToken": "d5b2b1a8-3e21-4a23-9f4b-87b0f6e4..."
    ///     }
    ///
    /// Example invalid or expired code:
    ///
    ///     {
    ///       "success": false,
    ///       "message": "The provided verification code is invalid or expired.",
    ///       "error": "InvalidCode"
    ///     }
    ///
    /// Example user not found:
    ///
    ///     {
    ///       "success": false,
    ///       "message": "User not found.",
    ///       "error": "UserNotFound"
    ///     }
    ///
    /// Possible values of <c>error</c>:
    /// - <c>None</c>
    /// - <c>InvalidCode</c>
    /// - <c>RateLimited</c>
    /// - <c>UserNotFound</c>
    /// - <c>NotEnabled</c>
    /// </remarks>
    /// <param name="dto">Payload containing the TOTP code used during login.</param>
    /// <response code="200">
    /// 2FA verified and the login flow can proceed (access and refresh tokens are issued).
    /// </response>
    /// <response code="400">Invalid request or user state (2FA not enabled).</response>
    /// <response code="401">Invalid or expired TOTP code.</response>
    /// <response code="404">User not found.</response>
    /// <response code="429">Too many failed attempts (rate limited).</response>
    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyLogin([FromBody] TwoFAVerifyLoginRequest dto)
    {
        if (!_twoFactorLoginSessionStore.TryGetUserId(dto.TwoFactorToken, out var userId))
        {
            return BadRequest(new { message = "Invalid or expired two-factor token." });
        }

        var res = await _svc.VerifyLoginAsync(userId, dto.Code);

        if (!res.Success)
        {
            return Unauthorized(new { message = res.Message ?? "Invalid or expired code. Please try again." });
        }

        var authResult = await _authService.IssueTokensAsync(userId);

        HttpContext.Response.Cookies.Append(
            "refreshToken",
            authResult.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = authResult.ExpiresAt
            });

        _twoFactorLoginSessionStore.Invalidate(dto.TwoFactorToken);

        return Ok(new Identity.API.DTO.Auth.LoginResponseDto
        {
            AccessToken = authResult.AccessToken,
            TokenType = "Bearer",
            RequiresTwoFactor = false,
            TwoFactorToken = null
        });
    }
}
