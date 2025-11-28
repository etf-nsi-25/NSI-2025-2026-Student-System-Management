using System.Collections.Generic;
using System;

namespace Identity.Core.DTO;

public class UserListResponse
{
    public IReadOnlyList<UserResponse> Items { get; init; } = new List<UserResponse>();
    
    public int TotalCount { get; init; }

    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}