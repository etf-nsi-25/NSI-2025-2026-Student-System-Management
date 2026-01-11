using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Faculty.API.Controllers
{
    [ApiController]
    [Route("api/exams")]
    [Authorize(Roles = "Teacher")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ILogger<ExamController> _logger;

        public ExamController(
            IExamService examService,
            ITeacherRepository teacherRepository,
            ILogger<ExamController> logger)
        {
            _examService = examService;
            _teacherRepository = teacherRepository;
            _logger = logger;
        }

        private async Task<Teacher> GetCurrentTeacherAsync()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }

            var teacher = await _teacherRepository.GetByUserIdAsync(userId);
            if (teacher == null)
            {
                throw new UnauthorizedAccessException("Teacher not found for the current user.");
            }

            return teacher;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ExamResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateExam([FromBody] CreateExamRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var teacher = await GetCurrentTeacherAsync();
                var result = await _examService.CreateExamAsync(request, teacher.Id, teacher.FacultyId);

                return CreatedAtAction(nameof(GetExamById), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized exam creation attempt");
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exam");
                return StatusCode(500, new { message = "An error occurred while creating the exam." });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ExamResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetExams()
        {
            try
            {
                var teacher = await GetCurrentTeacherAsync();
                var exams = await _examService.GetExamsByTeacherAsync(teacher.Id);
                return Ok(exams);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized exam list access attempt");
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exams");
                return StatusCode(500, new { message = "An error occurred while retrieving exams." });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExamResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetExamById(int id)
        {
            try
            {
                var teacher = await GetCurrentTeacherAsync();
                var exam = await _examService.GetExamByIdAsync(id, teacher.Id);

                if (exam == null)
                    return NotFound();

                return Ok(exam);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized exam access attempt for ID {ExamId}", id);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam {ExamId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the exam." });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ExamResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateExam(int id, [FromBody] UpdateExamRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var teacher = await GetCurrentTeacherAsync();
                var result = await _examService.UpdateExamAsync(id, request, teacher.Id);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized exam update attempt for ID {ExamId}", id);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exam {ExamId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the exam." });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteExam(int id)
        {
            try
            {
                var teacher = await GetCurrentTeacherAsync();
                var result = await _examService.DeleteExamAsync(id, teacher.Id);

                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized exam deletion attempt for ID {ExamId}", id);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exam {ExamId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the exam." });
            }
        }
    }
}