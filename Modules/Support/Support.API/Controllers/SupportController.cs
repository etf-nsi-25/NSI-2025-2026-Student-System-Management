using Faculty.Infrastructure.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Support.Application.DTOs;
using Support.Core.Entities;
using Support.Infrastructure.Db;
using Support.Infrastructure.Services;
using University.Infrastructure.Db;

namespace Support.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupportController : ControllerBase
{
    private readonly SupportDbContext _dbContext;
    private readonly UniversityDbContext _universityDbContext;
    private readonly FacultyDbContext _facultyDbContext;
    private readonly IDocumentPdfGenerator _pdfGenerator;

    public SupportController(
        SupportDbContext dbContext,
        UniversityDbContext universityDbContext,
        FacultyDbContext facultyDbContext,
        IDocumentPdfGenerator pdfGenerator)
    {
        _dbContext = dbContext;
        _universityDbContext = universityDbContext;
        _facultyDbContext = facultyDbContext;
        _pdfGenerator = pdfGenerator;
    }

   
    [Authorize]
    [HttpPost("document-request")]
    public async Task<IActionResult> CreateDocumentRequest([FromBody] CreateDocumentRequestDTO dto)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim))
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);

        var studentExists = await _facultyDbContext.Students
            .IgnoreQueryFilters()
            .AnyAsync(s =>
                s.UserId == userId.ToString() &&
                s.FacultyId == dto.FacultyId
            );

        if (!studentExists)
            return BadRequest("Student does not exist.");

        var facultyExists = await _universityDbContext.Faculties
            .AnyAsync(f => f.Id == dto.FacultyId);

        if (!facultyExists)
            return BadRequest("Faculty does not exist.");

       var request = new DocumentRequest
{
    Id = Guid.NewGuid(),
    UserId = userId,
    FacultyId = dto.FacultyId,
    DocumentType = dto.DocumentType,
    Status = "Pending",
    CreatedAt = DateTime.UtcNow,
    CompletedAt = null,
    Payload = dto.Payload
};


        _dbContext.DocumentRequests.Add(request);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            request.Id,
            request.Status
        });
    }

    
    [Authorize(Roles = "Admin")]
    [HttpGet("document-requests")]
    public async Task<IActionResult> GetAllDocumentRequests()
    {
        var items = await _dbContext.DocumentRequests
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.Id,
                r.UserId,
                r.FacultyId,
                r.DocumentType,
                r.Status,
                r.CreatedAt
            })
            .ToListAsync();

        return Ok(items);
    }

   
    [Authorize(Roles = "Admin")]
    [HttpPut("document-requests/{id:guid}/approve")]
    public async Task<IActionResult> ApproveDocumentRequest(Guid id)
    {
        var request = await _dbContext.DocumentRequests
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
            return NotFound();

        if (request.Status != "Pending")
            return BadRequest($"Only Pending requests can be approved. Current status: {request.Status}");

        request.Status = "Approved";
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            request.Id,
            request.Status
        });
    }

   
    [Authorize]
    [HttpGet("document-requests/{id:guid}/pdf")]
    public async Task<IActionResult> DownloadPdf(Guid id)
    {
        var request = await _dbContext.DocumentRequests
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
            return NotFound();

        if (request.Status != "Approved")
            return BadRequest("Request is not approved.");

        var pdfBytes = _pdfGenerator.Generate(
            "Student Confirmation",
            $"Request ID: {request.Id}\nUser: {request.UserId}\nFaculty: {request.FacultyId}"
        );

        return File(
            pdfBytes,
            "application/pdf",
            $"document-{request.Id}.pdf"
        );
    }
}
