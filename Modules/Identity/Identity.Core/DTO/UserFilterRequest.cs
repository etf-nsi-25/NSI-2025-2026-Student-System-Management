using Identity.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Identity.Core.DTO;

public class UserFilterRequest
{
    public Guid? FacultyId { get; set; }
    public UserRole? Role { get; set; }

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;

    public string? SortBy { get; set; } 
    public string SortOrder { get; set; } = "asc"; 
}