using Microsoft.AspNetCore.Mvc;

namespace University.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new JsonResult("Hello from University API!"));
    }
}