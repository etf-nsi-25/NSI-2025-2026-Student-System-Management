using System;
using System.Threading.Tasks;
using Identity.Application.DTO;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class TwoFactorAuthController : ControllerBase
{
    private readonly ITwoFactorAuthService _svc;

    public TwoFactorAuthController(ITwoFactorAuthService svc)
    {
        _svc = svc;
    }

    [HttpPost("enable-2fa")]
    public async Task<IActionResult> Enable([FromBody] TwoFAConfirmRequest dto)
    {
        var res = await _svc.EnableTwoFactorAsync(dto.UserId);
        return Ok(res);
    }

    [HttpPost("enable-2fa/confirm")]
    public async Task<IActionResult> VerifySetup([FromBody] TwoFAConfirmRequest dto)
    {
        var res = await _svc.VerifySetupAsync(dto.UserId, dto.Code);

        if (!res.Success)
            return BadRequest(res);

        return Ok(res);
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyLogin([FromBody] TwoFAConfirmRequest dto)
    {
        var res = await _svc.VerifyLoginAsync(dto.UserId, dto.Code);

        if (!res.Success)
            return BadRequest(res);

        return Ok(res);
    }
}
