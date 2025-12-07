using System.ComponentModel.DataAnnotations;

namespace University.Application.DTOs
{
    public class CreateFacultyDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
