using Faculty.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Faculty.API.Controllers
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _service;

        public StatisticsController(IStatisticsService service)
        {
            _service = service;
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            int count = await _service.GetStudentsCountAsync();
            return Ok(new { students = count });
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            int count = await _service.GetEmployeesCountAsync();
            return Ok(new { employees = count });
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses()
        {
            int count = await _service.GetCoursesCountAsync();
            return Ok(new { courses = count });
        }

        [HttpGet("activities")]
        public async Task<IActionResult> GetActivities()
        {
            int count = await _service.GetActivitiesCountAsync();
            return Ok(new { activities = count });
        }
    }
}
