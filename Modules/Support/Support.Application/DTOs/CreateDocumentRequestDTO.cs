namespace Support.Application.DTOs;

public class CreateDocumentRequestDTO
{
    public Guid FacultyId { get; set; }
    public string DocumentType { get; set; } = null!;
    public string Payload { get; set; } = "{}";
}
