using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faculty.API.Controllers;

[ApiController]
[Authorize(Roles = "Teacher")]
[Route("api/grades")]
public class StudentExamGradeController : ControllerBase
{
    private readonly IStudentExamGradeService _service;

    public StudentExamGradeController(IStudentExamGradeService service)
    {
        _service = service;
    }

    private int TeacherId => int.Parse(User.FindFirst("userId")!.Value);

    [HttpGet("exams/{examId}")]
    public async Task<IActionResult> GetAllGrades(int examId, CancellationToken ct)
    {
        var result = await _service.GetAllForExamAsync(examId, TeacherId, ct);
        return Ok(result);
    }

    [HttpPost("exams/{examId}/students/{studentId}")]
    public async Task<IActionResult> CreateOrUpdate(
        int examId,
        int studentId,
        [FromBody] GradeRequestDTO requestDto,
        CancellationToken ct)
    {
        var result = await _service.CreateOrUpdateAsync(
            examId,
            studentId,
            requestDto,
            TeacherId,
            ct);

        return Ok(result);
    }

    [HttpPut("exams/{examId}/students/{studentId}")]
    public async Task<IActionResult> Update(
        int examId,
        int studentId,
        [FromBody] GradeUpdateRequestDTO requestDto,
        CancellationToken ct)
    {
        var result = await _service.UpdateAsync(
            studentId,
            examId,
            requestDto,
            TeacherId,
            ct);

        return Ok(result);
    }

    [HttpDelete("exams/{examId}/students/{studentId}")]
    public async Task<IActionResult> Delete(int examId, int studentId, CancellationToken ct)
    {
        await _service.DeleteAsync(studentId, examId, TeacherId, ct);
        return NoContent();
    }
}