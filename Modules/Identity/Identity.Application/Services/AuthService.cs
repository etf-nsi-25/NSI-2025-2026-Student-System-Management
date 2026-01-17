using Identity.Core.Interfaces.Repositories;
using Identity.Core.Interfaces.Services;
using Identity.Core.Models;
using Microsoft.Extensions.Logging;
using Identity.Core.Enums;

namespace Identity.Application.Services;

public class AuthService : IAuthService
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IIdentityService _identityService; 
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IJwtTokenService jwtTokenService,
        IIdentityService identityService, 
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<AuthService> logger)
    {
        _jwtTokenService = jwtTokenService;
        _identityService = identityService;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

        public async Task<AuthResult> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Authentication attempt for email: {Email}", email);

        var user = await _identityService.FindByEmailAsync(email);

        if (user == null) 
        {
            _logger.LogWarning("Authentication failed: User not found - {Email}", email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var isPasswordValid = await _identityService.CheckPasswordAsync(user.Id, password);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Authentication failed: Invalid password - {Email}", email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (user.Status == UserStatus.Inactive) 
        {
            _logger.LogWarning("Authentication failed: User is deactivated - {Email}", email);
            throw new UnauthorizedAccessException("User is deactivated");
        }

        var tokenClaims = new TokenClaims
        {
            UserId = user.Id, 
            Email = user.Email, 
            Role = user.Role,
            TenantId = user.FacultyId.ToString(), 
            FullName = $"{user.FirstName} {user.LastName}".Trim()
        };

        var accessToken = _jwtTokenService.GenerateAccessToken(tokenClaims);
        var refreshToken = _jwtTokenService.CreateRefreshToken(user.Id);

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        _logger.LogInformation("Authentication successful for user: {UserId}", user.Id);

        return new AuthResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt
        };
    }

    public async Task<AuthResult> RefreshAuthenticationAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Token refresh attempt");

        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

        if (token == null || !token.IsActive)
        {
            _logger.LogWarning("Token refresh failed: Invalid or expired token");
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = await _identityService.FindByIdAsync(token.UserId);

        if (user == null)
        {
            _logger.LogWarning("Token refresh failed: User not found - {UserId}", token.UserId);
            throw new UnauthorizedAccessException("User not found or inactive");
        }

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedReason = "Replaced by new token";
        await _refreshTokenRepository.UpdateAsync(token, cancellationToken);

        var tokenClaims = new TokenClaims
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role,
            TenantId = user.FacultyId.ToString() ?? string.Empty,
            FullName = $"{user.FirstName ?? ""} {user.LastName ?? ""}".Trim()
        };

        var accessToken = _jwtTokenService.GenerateAccessToken(tokenClaims);
        var newRefreshToken = _jwtTokenService.CreateRefreshToken(user.Id);

        token.ReplacedByToken = newRefreshToken.Token;
        await _refreshTokenRepository.UpdateAsync(token, cancellationToken);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        _logger.LogInformation("Token refresh successful for user: {UserId}", user.Id);

        return new AuthResult
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = newRefreshToken.ExpiresAt
        };
    }

    public async Task RevokeAuthenticationAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Logout attempt");

        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

        if (token != null && token.IsActive)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedReason = "User logout";
            await _refreshTokenRepository.UpdateAsync(token, cancellationToken);
        }

        _logger.LogInformation("Logout successful");
    }
}