using Faculty.Infrastructure.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Support.Application.DTOs;
using Support.Core.Entities;
using Support.Infrastructure.Db;
using University.Infrastructure.Db;
using Microsoft.AspNetCore.Authorization;

namespace Support.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportController : ControllerBase
{
    private readonly SupportDbContext _dbContext;
    // private readonly UniversityDbContext _universityDbContext; 
    // private readonly FacultyDbContext _facultyDbContext;       

    public SupportController(
        SupportDbContext dbContext 
        /*, UniversityDbContext universityDbContext, */ 
        /*, FacultyDbContext facultyDbContext */         
    )
    {
        _dbContext = dbContext;
        // _universityDbContext = universityDbContext; 
        // _facultyDbContext = facultyDbContext;       
    }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get() => Ok("Support API is running with optimized DI.");

        // [HttpPost("document-request")]
        // public async Task<IActionResult> CreateDocumentRequest([FromBody] CreateDocumentRequestDTO dto)
        // {

        //     var studentExists = await _facultyDbContext.Students
        //         .AnyAsync(s => s.UserId == dto.UserId);

        //     if (!studentExists)
        //     {
        //         return BadRequest($"User/Student with ID '{dto.UserId}' does not exist.");
        //     }

        //     var facultyExists = await _universityDbContext.Faculties
        //         .AnyAsync(f => f.Id == dto.FacultyId);

        //     if (!facultyExists)
        //     {
        //         return BadRequest($"Faculty with ID '{dto.FacultyId}' does not exist.");
        //     }

        //     var request = new DocumentRequest
        //     {
        //         UserId = dto.UserId,
        //         FacultyId = dto.FacultyId,
        //         DocumentType = dto.DocumentType,
        //         Status = dto.Status,
        //         CreatedAt = DateTime.UtcNow,
        //         CompletedAt = null
        //     };

        //     _dbContext.DocumentRequests.Add(request);
        //     await _dbContext.SaveChangesAsync();

        //     return Ok(request);
        // }

        [HttpPost("document-request")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateDocumentRequest(
            [FromBody] CreateDocumentRequestDTO dto,
            [FromServices] FacultyDbContext facultyDbContext,     
            [FromServices] UniversityDbContext universityDbContext 
        )
        {

            var studentExists = await facultyDbContext.Students
                .AnyAsync(s => s.UserId == dto.UserId);

            if (!studentExists)
                return BadRequest($"User/Student with ID '{dto.UserId}' does not exist.");

            var facultyExists = await universityDbContext.Faculties
                .AnyAsync(f => f.Id == dto.FacultyId);

            if (!facultyExists)
                return BadRequest($"Faculty with ID '{dto.FacultyId}' does not exist.");

            var request = new DocumentRequest
            {
                UserId = dto.UserId,
                FacultyId = dto.FacultyId,
                DocumentType = dto.DocumentType,
                Status = dto.Status ?? "Pending",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = null
            };

            _dbContext.DocumentRequests.Add(request);
            await _dbContext.SaveChangesAsync();

            return Ok(request);
        }

        [HttpGet("requests")]
        [AllowAnonymous]
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

        [HttpPatch("requests/{id}/status")]
        [AllowAnonymous]
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