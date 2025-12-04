using Microsoft.AspNetCore.Mvc;
using Support.Application.DTOs;
using Support.Core.Entities;
using Support.Infrastructure.Db;

namespace Support.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportController : ControllerBase
    {
		private readonly SupportDbContext _dbContext;
		public SupportController(SupportDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpGet]
        public IActionResult Get() => Ok("Hello from Support API!");

		[HttpPost("document-request")]
		public async Task<IActionResult> CreateDocumentRequest([FromBody] CreateDocumentRequestDTO dto)
		{
			var request = new DocumentRequest
			{
				UserId = dto.UserId,
				FacultyId = dto.FacultyId,
				DocumentType = dto.DocumentType,
				Status = dto.Status,
				CreatedAt = DateTime.UtcNow,
				CompletedAt = null
			};

			_dbContext.DocumentRequests.Add(request);
			await _dbContext.SaveChangesAsync();

			return Ok(request);
		}



	}
}
