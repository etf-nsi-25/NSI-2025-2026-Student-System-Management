using System.ComponentModel.DataAnnotations;
using Faculty.Core.Enums;

namespace Faculty.Application.DTOs
{
    public class UpdateFacultyDto
    {
        [Required(ErrorMessage = "Faculty name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(20, ErrorMessage = "Abbreviation cannot exceed 20 characters")]
        public string? Abbreviation { get; set; }
        
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
        
        public FacultyStatus Status { get; set; }
    }
}