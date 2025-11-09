using System.ComponentModel.DataAnnotations;

namespace Identity.API.DTO;

public class CreateUserRequest
{
    [Required]
    public string Email { get; set; }
}