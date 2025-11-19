using Microsoft.AspNetCore.Mvc;
using Support.Application.DTOs;
using Support.Application.Services;
using Support.Core.Interfaces;

namespace Support.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _service;

        public RequestsController(IRequestService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequestDto dto)
        {
            var result = await _service.CreateRequestAsync(dto);
            return Ok(result);
        }
    }
}
