using Identity.Core.Interfaces.Services;
using Identity.Infrastructure.Entities; 
using Microsoft.AspNetCore.Identity;
using Identity.Core.DTO; 
using Microsoft.EntityFrameworkCore; 
using Identity.Infrastructure.Mappers; 

namespace Identity.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserResponse?> FindByEmailAsync(string email)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        return appUser != null ? UserMapper.MapToResponse(appUser) : null;
    }

    public async Task<UserResponse?> FindByIdAsync(string userId)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        return appUser != null ? UserMapper.MapToResponse(appUser) : null;
    }

    public async Task<bool> CheckPasswordAsync(string userId, string password)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return false;
        
        return await _userManager.CheckPasswordAsync(appUser, password);
    }

    public async Task<(bool Success, string[] Errors)> CreateUserAsync(CreateUserRequest request, string password)
    {
        var appUser = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            FacultyId = request.FacultyId,
            Role = request.Role,
            IndexNumber = request.IndexNumber,
            Status = Identity.Core.Enums.UserStatus.Active 
        };

        var result = await _userManager.CreateAsync(appUser, password);
        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

        public async Task<bool> UpdateUserAsync(UpdateUserRequest request)
    {
        var appUser = await _userManager.FindByIdAsync(request.Id);
        if (appUser == null) return false;

        appUser.FirstName = request.FirstName;
        appUser.LastName = request.LastName;
        appUser.Email = request.Email;
        appUser.UserName = request.Username; 
        appUser.FacultyId = request.FacultyId;
        appUser.Role = request.Role;
        appUser.IndexNumber = request.IndexNumber;
        appUser.Status = request.Status;

        var result = await _userManager.UpdateAsync(appUser);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return false;

        var result = await _userManager.DeleteAsync(appUser);
        return result.Succeeded;
    }

    public async Task<IEnumerable<UserResponse>> GetAllFilteredAsync(UserFilterRequest filter)
{
    var query = _userManager.Users.AsQueryable();

    if (filter.FacultyId.HasValue)
        query = query.Where(u => u.FacultyId == filter.FacultyId);
    
    if (filter.Role.HasValue)
        query = query.Where(u => u.Role == filter.Role);

    var appUsers = await query
        .OrderBy(u => u.UserName) 
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .ToListAsync();

    return appUsers.Select(UserMapper.MapToResponse);
}

    public async Task<int> CountAsync(UserFilterRequest filter)
    {
        var query = _userManager.Users.AsQueryable();

        if (filter.FacultyId.HasValue)
            query = query.Where(u => u.FacultyId == filter.FacultyId);
        
        if (filter.Role.HasValue)
            query = query.Where(u => u.Role == filter.Role);

        return await query.CountAsync();
    }
}