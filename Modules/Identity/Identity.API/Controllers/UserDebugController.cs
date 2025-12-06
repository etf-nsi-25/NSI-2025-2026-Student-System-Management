using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Identity.Infrastructure.Entities;
using System.Threading.Tasks;

[ApiController]
[Route("api/debug-user")]
public class UserDebugController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserDebugController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateTestUser()
    {
        var user = new ApplicationUser
        {
            UserName = "hanaaa@test.com",
            Email = "hanaaa@test.com"
        };

        var result = await _userManager.CreateAsync(user, "Password123!");

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(user.Id);
    }
}
