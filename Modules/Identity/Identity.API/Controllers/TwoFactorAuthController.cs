using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Identity.Application.DTO;

[ApiController]
[Route("api/auth")]
public class  : ControllerBase
{
    private readonly ITwoFactorTwoFactorAuthControllerAuthService _svc;

    public TwoFactorAuthController(ITwoFactorAuthService svc)
    {
        _svc = svc;
    }

    [HttpPost("enable-2fa")]
    public async Task<IActionResult> Enable()
    {
        string userId = "11111111-1111-1111-1111-111111111111";
        var res = await _svc.EnableTwoFactorAsync(userId);
        return Ok(res);
    }

    [HttpPost("verify-2fa-setup")]
    public async Task<IActionResult> VerifySetup([FromBody] TwoFAConfirmRequest dto)
    {
        string userId = "11111111-1111-1111-1111-111111111111";
        var res = await _svc.VerifySetupAsync(userId, dto.Code);

        if (!res.Success)
            return BadRequest(res);

        return Ok(res);
    }

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
