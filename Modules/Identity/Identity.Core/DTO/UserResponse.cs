using System;
using Identity.Core.Enums; 

namespace Identity.Core.DTO; 

public class UserResponse
{
    public required string Id { get; set; }
    public required string Email { get; set; }

    public required string Username { get; set; } 

    public required string FirstName { get; set; } 

    public required string LastName { get; set; } 

    public string? IndexNumber { get; set; } 

    public required Guid FacultyId { get; set; } 

    public UserRole Role { get; set; } 

    public UserStatus Status { get; set; } 
    
    public bool ForcePasswordChange { get; set; }

    public bool TwoFactorEnabled { get; set; }
}