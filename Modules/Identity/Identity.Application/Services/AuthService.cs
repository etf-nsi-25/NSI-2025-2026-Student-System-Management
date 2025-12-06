
using Identity.Core.Entities;
using Identity.Core.Enums;
using Identity.Core.Interfaces.Repositories;
using Identity.Core.Interfaces.Services;
using Identity.Core.Models;
using Identity.Core.Repositories;
using Identity.Core.Services;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Services;

public class AuthService : IAuthService
{
    //private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IIdentityHasherService _passwordHasher;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IJwtTokenService jwtTokenService,
        IIdentityHasherService passwordHasher,
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ILogger<AuthService> logger)
    {
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<AuthResult> AuthenticateAsync(
        string email,
        string password,
        string ipAddress,
        string userAgent,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Authentication attempt for email: {Email}", email);
        
        var admin = User.Create("Amar Tahirovic", _passwordHasher.HashPassword(password), "Amar", "Tahirovic", email, Guid.NewGuid(), UserRole.Student, "19006");

       await _userRepository.AddAsync(admin);
       await _userRepository.SaveAsync();

        // Find user by email
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);


        if (user == null) 
        {
            _logger.LogWarning("Authentication failed: User not found or inactive - {Email}", email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            _logger.LogWarning("Authentication failed: Invalid password - {Email}", email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Generate tokens using domain model
        var tokenClaims = new TokenClaims
        {
            UserId = user.Id.ToString(),
            Email = user.Email,
            Role = user.Role,
            TenantId = user.FacultyId.ToString(),
            FullName = user.FirstName + " " + user.LastName
        };

        var accessToken = _jwtTokenService.GenerateAccessToken(tokenClaims);
        var refreshToken = _jwtTokenService.CreateRefreshToken(user.Id, ipAddress, userAgent);

        // Save refresh token to repository
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
        string ipAddress,
        string userAgent,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Token refresh attempt");

        // Validate refresh token
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

        if (token == null || !token.IsActive)
        {
            _logger.LogWarning("Token refresh failed: Invalid or expired token");
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        // Get user
        var user = await _userRepository.GetByIdAsync(token.UserId);




        if (user == null)
        {
            _logger.LogWarning("Token refresh failed: User not found or inactive - {UserId}", token.UserId);
            throw new UnauthorizedAccessException("User not found or inactive");
        }

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedReason = "Replaced by new token";
        await _refreshTokenRepository.UpdateAsync(token, cancellationToken);

        // Generate new tokens using domain model
        var tokenClaims = new TokenClaims
        {
            UserId = user.Id.ToString(),
            Email = user.Email,
            Role = user.Role,
            TenantId = user.FacultyId.ToString(),
            FullName = user.FirstName + " " + user.LastName
        };

        var accessToken = _jwtTokenService.GenerateAccessToken(tokenClaims);
        var newRefreshToken = _jwtTokenService.CreateRefreshToken(user.Id, ipAddress, userAgent);

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

    public Task<PublicKeyInfo> GetPublicKeyInfoAsync()
    {
        PublicKeyInfo publicKeyInfo = _jwtTokenService.GetPublicKey();
        return Task.FromResult(publicKeyInfo);
    }
}