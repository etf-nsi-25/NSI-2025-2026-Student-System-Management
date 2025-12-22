using Microsoft.AspNetCore.Mvc;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Faculty.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly FacultyDbContext _db;

        public DebugController(FacultyDbContext db)
        {
            _db = db;
        }

        [HttpGet("courses-count")]
        public async Task<IActionResult> GetCoursesCount()
        {
            // Ignoriše globalne filtere (tenant, soft delete, itd.)
            var count = await _db.Courses.IgnoreQueryFilters().CountAsync();
            return Ok(new { CoursesCount = count });
        }
    }
}
