using System.ComponentModel.DataAnnotations;
using Identity.Core.Enums;
using System;

namespace Identity.Core.DTO;

public class CreateUserRequest
{
    
    [Required]
    public required string Username { get; set; } 
    
    [Required]
    public required string Password { get; set; } 
    
    [Required]
    public required string FirstName { get; set; } 
    
    [Required]
    public required string LastName { get; set; } 
    
    [Required]
    public required Guid FacultyId { get; set; }
    
    public string? IndexNumber { get; set; } 
    
    [Required]
    public required UserRole Role { get; set; }
}