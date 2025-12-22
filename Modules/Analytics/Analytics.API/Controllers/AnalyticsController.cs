using Analytics.Application.DTOs;
using Analytics.Infrastructure;
using Faculty.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Analytics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnalyticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("university-metrics")]
        public async Task<IActionResult> GetUniversityMetrics()
        {
            var query = new GetUniversityMetricsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
