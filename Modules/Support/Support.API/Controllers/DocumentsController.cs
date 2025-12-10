using Microsoft.AspNetCore.Mvc;
using Support.Application.DTOs;

namespace Support.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        // Fake studenti – simulacija baze
        private static readonly List<int> Students = new() { 10, 15 };

        // Fake dokumenti – simulacija baze
        private static readonly List<DocumentDto> Documents = new()
        {
            new DocumentDto(1, "example.pdf", "Sample PDF document", 10),
            new DocumentDto(2, "transcript.pdf", "Transcript document", 10),
            new DocumentDto(3, "enrollment.pdf", "Enrollment confirmation", 15)
        };

        // -------------------------------------------------------------
        // GET /api/documents
        // -------------------------------------------------------------
        [HttpGet]
        public IActionResult GetDocuments()
        {
            return Ok(Documents);
        }

        // -------------------------------------------------------------
        // GET /api/documents/{id}/download?studentId=10
        // -------------------------------------------------------------
        [HttpGet("{id}/download")]
        public IActionResult DownloadDocument(int id, [FromQuery] int studentId)
        {
            try
            {
                // 1️⃣ Provjera da je studentId poslan
                if (studentId <= 0)
                    return BadRequest("Invalid studentId.");

                // 2️⃣ Provjera da li student postoji (Matejev zahtjev!)
                if (!Students.Contains(studentId))
                    return NotFound($"Student with ID {studentId} does not exist.");

                // 3️⃣ Provjera da li dokument postoji
                var document = Documents.FirstOrDefault(d => d.Id == id);
                if (document == null)
                    return NotFound($"Document with ID {id} does not exist.");

                // 4️⃣ Provjera vlasništva dokumenta
                if (document.StudentId != studentId)
                    return StatusCode(403, "You are not authorized to access this document.");

                // 5️⃣ Umjesto PDF-a, samo poruka (task to NE TRAŽI PDF)
                return Ok($"Document '{document.FileName}' is ready for download.");
            }
            catch
            {
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}