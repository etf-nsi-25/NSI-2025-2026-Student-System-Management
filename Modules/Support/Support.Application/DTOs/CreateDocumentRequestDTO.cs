using System.ComponentModel.DataAnnotations;

namespace Support.Application.DTOs
{
    public class CreateDocumentRequestDTO
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; } = string.Empty;
        public Guid FacultyId { get; set; }

        [Required(ErrorMessage = "DocumentType is required.")]
        [StringLength(100, ErrorMessage = "DocumentType cannot exceed 100 characters.")]
        public string DocumentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("Pending|Approved|Rejected",
            ErrorMessage = "Status must be Pending, Approved, or Rejected.")]
        public string Status { get; set; } = string.Empty;
    }
}