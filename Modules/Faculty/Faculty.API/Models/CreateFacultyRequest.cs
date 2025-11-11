using System.ComponentModel.DataAnnotations;

namespace Faculty.API.Models
{
  public class CreateFacultyRequest
  {
    [Required(ErrorMessage = "Faculty name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Faculty name must be between 3 and 100 characters.")]
    public string? FacultyName { get; set; }
  }
}
