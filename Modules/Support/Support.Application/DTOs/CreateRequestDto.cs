using System;

namespace Support.Application.DTOs
{
    public class CreateRequestDto
{
    public Guid FacultyId { get; set; }
    public string DocumentType { get; set; } = null!;
    public object? Payload { get; set; }
}

}
