using Identity.Core.Interfaces.Services;
using Identity.Infrastructure.Entities; 
using Microsoft.AspNetCore.Identity;
using Identity.Core.DTO; 
using Microsoft.EntityFrameworkCore; 
using Identity.Infrastructure.Mappers; 
using System.Text.Encodings.Web;
using Npgsql;
using QRCoder;

namespace Identity.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UrlEncoder _urlEncoder;

    public IdentityService(UserManager<ApplicationUser> userManager, UrlEncoder urlEncoder)
    {
        _userManager = userManager;
        _urlEncoder = urlEncoder;
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

    public async Task<bool> IsTwoFactorEnabledAsync(string userId)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        return appUser?.TwoFactorEnabled ?? false;
    }

    public async Task<TwoFactorSetupInfo> GenerateTwoFactorSetupAsync(string userId, string issuer)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Ensure a fresh key each time setup is started.
        try
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg
                                           && pg.SqlState == "23505"
                                           && pg.ConstraintName == "PK_AspNetUserTokens")
        {
            // If the UI triggers /enable-2fa twice (common in React StrictMode),
            // both requests can race to insert the same AspNetUserTokens row.
            // In that case, just continue and read whichever key is currently stored.
        }

        // Reload to ensure we read the latest authenticator key if another request won the race.
        user = await _userManager.FindByIdAsync(userId) ?? user;

        var key = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("Failed to generate authenticator key");
        }

        var otpAuthUri = GenerateOtpAuthUri(issuer, user.Email ?? user.UserName ?? string.Empty, key);
        var qrCodeDataUri = GenerateQrCodeDataUri(otpAuthUri);

        return new TwoFactorSetupInfo(
            ManualKey: key,
            QrCodeImageBase64: qrCodeDataUri,
            OtpAuthUri: otpAuthUri);
    }

    public async Task<bool> ConfirmTwoFactorSetupAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultAuthenticatorProvider,
            code);

        if (!isValid)
        {
            return false;
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        return true;
    }

    public async Task<bool> VerifyTwoFactorCodeAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        if (!user.TwoFactorEnabled)
        {
            return false;
        }

        return await _userManager.VerifyTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultAuthenticatorProvider,
            code);
    }

    private string GenerateOtpAuthUri(string issuer, string email, string key)
    {
        // Matches the format used by common authenticator apps.
        // otpauth://totp/{issuer}:{email}?secret={key}&issuer={issuer}&digits=6
        var encodedIssuer = _urlEncoder.Encode(issuer);
        var encodedEmail = _urlEncoder.Encode(email);
        var encodedKey = _urlEncoder.Encode(key);

        return $"otpauth://totp/{encodedIssuer}:{encodedEmail}?secret={encodedKey}&issuer={encodedIssuer}&digits=6";
    }

    private static string GenerateQrCodeDataUri(string payload)
    {
        var generator = new QRCodeGenerator();
        var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(data);
        var pngBytes = qrCode.GetGraphic(20);
        return $"data:image/png;base64,{Convert.ToBase64String(pngBytes)}";
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
            Status = Identity.Core.Enums.UserStatus.Active,
            ForcePasswordChange = true
        };

        var result = await _userManager.CreateAsync(appUser, password);
        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<bool> UpdateUserAsync(UpdateUserRequest request)
    {
        var appUser = await _userManager.FindByIdAsync(request.Id);
        if (appUser == null) return false;
        
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            var resetResult = await _userManager.ResetPasswordAsync(appUser, token, request.Password);

            if (!resetResult.Succeeded)
            {
                return false;
            }
        }

        appUser.FirstName = request.FirstName;
        appUser.LastName = request.LastName;
        appUser.Email = request.Email;
        appUser.UserName = request.Username;
        appUser.FacultyId = request.FacultyId;
        appUser.Role = request.Role;
        appUser.IndexNumber = request.IndexNumber;
        appUser.Status = request.Status;
        appUser.ForcePasswordChange = request.ForcePasswordChange; 

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