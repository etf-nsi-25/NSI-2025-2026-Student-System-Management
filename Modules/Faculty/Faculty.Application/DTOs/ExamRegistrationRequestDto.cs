using System.ComponentModel.DataAnnotations;

namespace Faculty.Application.DTOs;

public class ExamRegistrationRequestDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ExamId must be greater than zero.")]
    public int ExamId { get; set; }
}
