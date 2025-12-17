using System.ComponentModel.DataAnnotations;

namespace Support.Application.DTOs
{
    public class UpdateIssueDto
    {
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
        public string? Subject { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a valid positive number")]
        public int? CategoryId { get; set; }

        public int? Status { get; set; }
    }
}
