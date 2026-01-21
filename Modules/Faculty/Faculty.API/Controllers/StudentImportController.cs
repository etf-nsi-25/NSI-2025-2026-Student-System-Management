using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/faculty/students/import")]
[Authorize(Roles = "Admin")]
public class StudentsImportController : ControllerBase
{
    private readonly IStudentImportService _service;

    public StudentsImportController(IStudentImportService service)
    {
        _service = service;
    }

    [HttpPost("preview")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Preview([FromForm] StudentImportRequestDTO request)
    {
        if (request.File == null)
            return BadRequest("File is required");

        var ext = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (ext != ".csv" && ext != ".xlsx")
            return StatusCode(StatusCodes.Status415UnsupportedMediaType);

        var facultyId = await _service.ResolveFacultyId(User);

        var students = await _service.ConvertFile(request.File, facultyId);
        await _service.ValidatePreview(students);

        return Ok(students);
    }

    [HttpPost("commit")]
    public async Task<IActionResult> Commit([FromBody] List<StudentImportDTO> students)
    {
        var facultyId = await _service.ResolveFacultyId(User);
        await _service.ValidatePreview(students);
        foreach (var student in students)
        {
            if (student.Errors.Any())
                return Ok(students);
        }
        await _service.Commit(students, facultyId);

        return Ok();
    }
}