using Faculty.Infrastructure.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Support.Application.DTOs;
using Support.Core.Entities;
using Support.Infrastructure.Db;
using University.Infrastructure.Db;

namespace Support.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportController : ControllerBase
    {
		private readonly SupportDbContext _dbContext;
		private readonly UniversityDbContext _universityDbContext;
		private readonly FacultyDbContext _facultyDbContext;
		public SupportController(SupportDbContext dbContext, UniversityDbContext universityDbContext, FacultyDbContext facultyDbContext)
		{
			_dbContext = dbContext;
			_universityDbContext = universityDbContext;
			_facultyDbContext = facultyDbContext;
		}

		[HttpGet]
        public IActionResult Get() => Ok("Hello from Support API!");

		[HttpPost("document-request")]
		public async Task<IActionResult> CreateDocumentRequest([FromBody] CreateDocumentRequestDTO dto)
		{

			var studentExists = await _facultyDbContext.Students
				.AnyAsync(s => s.UserId == dto.UserId); 

			if (!studentExists)
			{
				return BadRequest($"User/Student with ID '{dto.UserId}' does not exist.");
			}

			var facultyExists = await _universityDbContext.Faculties
				.AnyAsync(f => f.Id == dto.FacultyId);

			if (!facultyExists)
			{
				return BadRequest($"Faculty with ID '{dto.FacultyId}' does not exist.");
			}

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
