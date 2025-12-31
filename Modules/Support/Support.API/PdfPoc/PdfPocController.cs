using Microsoft.AspNetCore.Mvc;

namespace Support.API.PdfPoc;

[ApiController]
[Route("api/pdf-poc")]
public class PdfPocController : ControllerBase
{
    private readonly PdfPocService _service;

    public PdfPocController(PdfPocService service)
    {
        _service = service;
    }

    [HttpPost("student-confirmation")]
    public IActionResult StudentConfirmation([FromBody] StudentConfirmationData data)
    {
        var pdfBytes = _service.GenerateStudentConfirmationPdf(data);

        var fileName = $"student-confirmation-{DateTime.Now:yyyyMMdd-HHmmss}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }
}
