using System;
using Support.Core.Enums;

namespace Support.Core.Entities
{
public class DocumentRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FacultyId { get; set; }
    public string DocumentType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Payload { get; set; } = null!;
}


}