using System.ComponentModel.DataAnnotations;

namespace Support.Application.DTOs
{
    public class CreateIssueCategoryDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "Priority must be between 1 and 100")]
        public int Priority { get; set; } = 1;
    }
}
