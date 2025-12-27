using Microsoft.AspNetCore.Mvc;

namespace Support.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaqsController : ControllerBase
    {
        // Hardkodirana lista FAQ-a
        private static readonly List<object> Faqs = new()
        {
            new { id = 1, question = "How do I check my attendance?", answer = "Go to Student Dashboard → Attendance section to view your attendance records." },
            new { id = 2, question = "Where can I download my transcript?", answer = "Navigate to Document Center → Academic Documents and download your transcript." },
            new { id = 3, question = "Where should I pay the tuition?", answer = "Tuition payments can be made via the Student Portal → Payments section or directly at the university finance office." },
            new { id = 4, question = "Where can I see my grades?", answer = "Check the Student Dashboard → Academic Records to see all your grades." }
        };

        // GET /api/faqs
        [HttpGet]
        public IActionResult GetFaqs()
        {
            return Ok(Faqs);
        }
    }
}
