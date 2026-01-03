using Faculty.Infrastructure.Db;
using Microsoft.AspNetCore.Authorization;
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

		public SupportController(
			SupportDbContext dbContext,
			UniversityDbContext universityDbContext,
			FacultyDbContext facultyDbContext)
		{
			_dbContext = dbContext;
			_universityDbContext = universityDbContext;
			_facultyDbContext = facultyDbContext;
		}

		[HttpPost("document-request")]
		[Authorize]
		public async Task<IActionResult> CreateDocumentRequest([FromBody] CreateDocumentRequestDTO dto)
		{
			var studentExists = await _facultyDbContext.Students
				.AnyAsync(s => s.UserId == dto.UserId);

			if (!studentExists)
				return BadRequest($"User/Student with ID '{dto.UserId}' does not exist.");

			var facultyExists = await _universityDbContext.Faculties
				.AnyAsync(f => f.Id == dto.FacultyId);

			if (!facultyExists)
				return BadRequest($"Faculty with ID '{dto.FacultyId}' does not exist.");

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

		[HttpPost("enrollment-requests")]
		public async Task<IActionResult> CreateEnrollmentRequest([FromBody] CreateEnrollmentRequestDTO dto)
		{
			if (dto.Semester is not (1 or 2))
				return BadRequest("Semester must be 1 or 2.");

			if (string.IsNullOrWhiteSpace(dto.AcademicYear))
				return BadRequest("AcademicYear is required.");

			var studentExists = await _facultyDbContext.Students
				.AnyAsync(s => s.UserId == dto.UserId);

			if (!studentExists)
				return BadRequest($"Student with UserId '{dto.UserId}' does not exist.");

			/*var facultyExists = await _universityDbContext.Faculties
				.AnyAsync(f => f.Id.Equals(dto.FacultyId));

			if (!facultyExists)
				return BadRequest($"Faculty with ID '{dto.FacultyId}' does not exist.");*/

			var duplicate = await _dbContext.EnrollmentRequests.AnyAsync(r =>
				r.UserId == dto.UserId &&
				r.AcademicYear == dto.AcademicYear &&
				r.Semester == dto.Semester &&
				r.Status != "Rejected"
			);

			if (duplicate)
				return BadRequest("You already have an enrollment request for the same academic year and semester.");

			var request = new EnrollmentRequest
			{
				Id = Guid.NewGuid(),
				UserId = dto.UserId,
				FacultyId = dto.FacultyId,
				AcademicYear = dto.AcademicYear.Trim(),
				Semester = dto.Semester,
				Status = "Pending",
				CreatedAt = DateTime.UtcNow,
				DecisionAt = null,
				DecidedByUserId = null,
				DecisionNote = null
			};

			_dbContext.EnrollmentRequests.Add(request);
			await _dbContext.SaveChangesAsync();

			return CreatedAtAction(nameof(GetEnrollmentRequestById), new { id = request.Id }, request);
		}

		[HttpGet("enrollment-requests/{id:guid}")]
		public async Task<IActionResult> GetEnrollmentRequestById([FromRoute] Guid id)
		{
			var req = await _dbContext.EnrollmentRequests.AsNoTracking()
				.FirstOrDefaultAsync(x => x.Id == id);

			return req is null ? NotFound() : Ok(req);
		}

		[HttpGet("enrollment-requests/my")]
		public async Task<IActionResult> GetMyEnrollmentRequests([FromQuery] string userId)
		{
			var items = await _dbContext.EnrollmentRequests.AsNoTracking()
				.Where(r => r.UserId == userId)
				.OrderByDescending(r => r.CreatedAt)
				.Select(r => new
				{
					r.Id,
					r.CreatedAt,
					r.AcademicYear,
					r.Semester,
					r.Status
				})
				.ToListAsync();

			return Ok(items);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("enrollment-requests/pending")]
		public async Task<IActionResult> GetPendingEnrollmentRequestsForFaculty([FromQuery] Guid facultyId)
		{
			var items = await _dbContext.EnrollmentRequests.AsNoTracking()
				.Where(r => r.FacultyId.Equals(facultyId) && r.Status == "Pending")
				.OrderBy(r => r.CreatedAt)
				.ToListAsync();

			return Ok(items);
		}

		[Authorize(Roles = "Admin")]
		[HttpPut("enrollment-requests/{id:guid}/approve")]
		public async Task<IActionResult> ApproveEnrollmentRequest([FromRoute] Guid id, [FromBody] DecideEnrollmentRequestDTO? dto)
		{
			var req = await _dbContext.EnrollmentRequests
				.FirstOrDefaultAsync(r => r.Id == id);

			if (req is null) return NotFound();

			if (req.Status != "Pending")
				return BadRequest($"Only Pending requests can be approved. Current status: {req.Status}");

			req.Status = "Approved";
			req.DecisionAt = DateTime.UtcNow;
			req.DecidedByUserId = dto?.AdminUserId;
			req.DecisionNote = dto?.Note;

			await _dbContext.SaveChangesAsync();
			return Ok(req);
		}

		[Authorize(Roles = "Admin")]
		[HttpPut("enrollment-requests/{id:guid}/reject")]
		public async Task<IActionResult> RejectEnrollmentRequest([FromRoute] Guid id, [FromBody] DecideEnrollmentRequestDTO? dto)
		{
			var req = await _dbContext.EnrollmentRequests
				.FirstOrDefaultAsync(r => r.Id == id);

			if (req is null) return NotFound();

			if (req.Status != "Pending")
				return BadRequest($"Only Pending requests can be rejected. Current status: {req.Status}");

			req.Status = "Rejected";
			req.DecisionAt = DateTime.UtcNow;
			req.DecidedByUserId = dto?.AdminUserId;
			req.DecisionNote = dto?.Note;

			await _dbContext.SaveChangesAsync();
			return Ok(req);
		}

        [HttpGet("requests")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRequests()
        {
            try
            {
                var requests = await _dbContext.DocumentRequests
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                var response = requests.Select(r => new 
                {
                    Id = r.Id.ToString(),
                    Date = r.CreatedAt,
                    StudentIndex = r.UserId,
                    RequestType = r.DocumentType,
                    RequestDetails = $"Request for {r.DocumentType}",
                    Status = r.Status
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("requests/{id}/status")] 
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
		{
			var request = await _dbContext.DocumentRequests.FindAsync(id);
			
			if (request == null) 
				return NotFound(new { message = $"Request with ID {id} not found." });

			request.Status = dto.Status;
			
			if (dto.Status == "Approved") 
				request.CompletedAt = DateTime.UtcNow;

			await _dbContext.SaveChangesAsync();
			return Ok(new { message = "Status updated successfully" });
		}
    }
}