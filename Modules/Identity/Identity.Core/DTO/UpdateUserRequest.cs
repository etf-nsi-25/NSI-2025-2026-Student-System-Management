using System;
using Identity.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Identity.Core.DTO;

public class UpdateUserRequest
{
    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Required] 
    public required string Email { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required Guid FacultyId { get; set; }

    public string? IndexNumber { get; set; }

    public UserStatus Status { get; set; }

    [Required] 
    public required UserRole Role { get; set; } 

    public string Id { get; set; } = string.Empty;
    public string? Password { get; set; }
}