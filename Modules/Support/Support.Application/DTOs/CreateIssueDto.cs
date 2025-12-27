using System.ComponentModel.DataAnnotations;

namespace Support.Application.DTOs
{
    public class CreateIssueDto
    {
        [Required(ErrorMessage = "Subject is required")]
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "CategoryId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a valid positive number")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; } = string.Empty;
    }
}
