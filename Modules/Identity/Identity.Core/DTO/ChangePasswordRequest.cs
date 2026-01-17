using System.ComponentModel.DataAnnotations;

namespace Identity.Core.DTO;

public class ChangePasswordRequest
{
    [Required]
    [MinLength(8)]
    public required string NewPassword { get; set; }
}
