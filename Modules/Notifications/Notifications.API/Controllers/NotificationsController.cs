using Microsoft.AspNetCore.Mvc;

namespace Notifications.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("Hello from Notifications API!");

    }
}
