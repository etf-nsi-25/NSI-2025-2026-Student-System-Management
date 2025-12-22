using System.ComponentModel.DataAnnotations;

namespace Support.Application.DTOs
{
    public class UpdateStatusDto
    {
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("Pending|Approved|Rejected", 
            ErrorMessage = "Status must be Pending, Approved, Rejected.")]
        public string Status { get; set; } = string.Empty;
    }
}