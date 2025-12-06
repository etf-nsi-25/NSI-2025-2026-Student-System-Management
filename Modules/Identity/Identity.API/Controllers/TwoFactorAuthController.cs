using System;
using System.Threading.Tasks;
using Identity.Application.DTO;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Identity.Application.DTO;

[ApiController]
[Route("api/auth")]
public class TwoFactorAuthController : ControllerBase
{
    private readonly ITwoFactorTwoFactorAuthControllerAuthService _svc;

    public TwoFactorAuthController(
        ITwoFactorAuthService svc,
        UserManager<ApplicationUser> userManager)
    {
        _svc = svc;
        _userManager = userManager;
    }

    // HELPERS ---------------------------------------------------------

    // 1) Uzmi POSLJEDNJEG usera u tabeli
    private async Task<string> GetLastUserIdAsync()
    {
        var user = await _userManager.Users
            .OrderByDescending(u => u.Id)
            .FirstOrDefaultAsync();

        if (user == null)
            throw new Exception("No users found in database.");

        return user.Id;
    }

    // 2) Vrati hardcoded usera (hanaa@test.com)
    private async Task<string> GetHanaaUserIdAsync()
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == "hanaa@test.com");

        if (user == null)
            throw new Exception("User 'hanaa@test.com' not found.");

        return user.Id;
    }

    // END HELPERS -----------------------------------------------------


    // STEP 1 — GENERATE SECRET + QR FOR LAST USER
    [HttpPost("enable-2fa")]
    public async Task<IActionResult> Enable([FromBody] TwoFAConfirmRequest dto)
    {
        var userId = await GetLastUserIdAsync();
        var res = await _svc.EnableTwoFactorAsync(userId);
        return Ok(res);
    }

    // STEP 2 — CONFIRM ENABLE FOR HARD-CODED USER hanaa@test.com
    [HttpPost("enable-2fa/confirm")]
    public async Task<IActionResult> ConfirmEnable([FromBody] TwoFAConfirmRequest dto)
    {
        string userId = "11111111-1111-1111-1111-111111111111";
        var res = await _svc.VerifySetupAsync(userId, dto.Code);

        if (!res.Success)
            return BadRequest(res);

        return Ok(res);
    }

    // STEP 3 — LOGIN VERIFICATION FOR HARD-CODED USER hanaa@test.com
    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyLogin([FromBody] TwoFAConfirmRequest dto)
    {
        string userId = "11111111-1111-1111-1111-111111111111";
        var res = await _svc.VerifyLoginAsync(userId, dto.Code);

        if (!res.Success)
            return BadRequest(res);

        return Ok(res);
    }
}
