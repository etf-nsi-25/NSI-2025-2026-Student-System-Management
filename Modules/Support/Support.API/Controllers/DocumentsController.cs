using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Support.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        // Dummy dokumenti — simulacija baze
        private static readonly List<DocumentDto> Documents = new()
        {
            new DocumentDto(1, "example.pdf", "Sample PDF document", StudentId: 10),
            new DocumentDto(2, "transcript.pdf", "Student transcript", StudentId: 10),
            new DocumentDto(3, "enrollment.pdf", "Enrollment confirmation", StudentId: 15)
        };

        // -------------------------------------------------------
        // TASK 328 — GET /api/documents
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult GetAvailableDocuments()
        {
            return Ok(Documents);
        }

        // -------------------------------------------------------
        // TASK 330 + TASK 329 + TASK 331
        // GET /api/documents/{id}/download
        // -------------------------------------------------------
        [HttpGet("{id}/download")]
        public IActionResult DownloadDocument(int id, [FromQuery] int studentId)
        {
            try
            {
                // TASK 331 — error handling: 400
                if (studentId <= 0)
                    return BadRequest("Invalid studentId");

                // TASK 331 — error handling: 404
                var document = Documents.Find(d => d.Id == id);
                if (document == null)
                    return NotFound("Document not found");

                // TASK 329 — VALIDATION (student ownership)
                if (document.StudentId != studentId)
                    return StatusCode(403, "You are not authorized to access this document");

                // TASK 330 — Generate dummy PDF
                byte[] pdfBytes = GenerateValidPdf();

                return File(
                    pdfBytes,
                    "application/pdf",
                    document.FileName
                );
            }
            catch (Exception)
            {
                // TASK 331 — error handling: 500
                return StatusCode(500, "Internal server error");
            }
        }

        // -------------------------------------------------------
        // Helper: valid PDF generator
        // -------------------------------------------------------
        private byte[] GenerateValidPdf()
        {
            string pdf = @"%PDF-1.4
1 0 obj
<< /Type /Catalog /Pages 2 0 R >>
endobj
2 0 obj
<< /Type /Pages /Kids [3 0 R] /Count 1 >>
endobj
3 0 obj
<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R >>
endobj
4 0 obj
<< /Length 44 >>
stream
BT
/F1 24 Tf
100 700 Td
(Hello from UNSA PDF!) Tj
ET
endstream
endobj
xref
0 5
0000000000 65535 f
0000000010 00000 n
0000000061 00000 n
0000000114 00000 n
0000000215 00000 n
trailer
<< /Size 5 /Root 1 0 R >>
startxref
330
%%EOF";

            return System.Text.Encoding.ASCII.GetBytes(pdf);
        }

        // DTO
        private record DocumentDto(int Id, string FileName, string Description, int StudentId);
    }
}
