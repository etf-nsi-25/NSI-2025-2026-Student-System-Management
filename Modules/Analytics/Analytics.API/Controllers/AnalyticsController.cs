using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("Hello from Analytics API!");

    }
}
