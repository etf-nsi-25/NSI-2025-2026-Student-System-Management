using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Identity.Core.Configuration;
using Identity.Core.Entities;
using Identity.Core.Interfaces.Services;
using Identity.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Application.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly SymmetricSecurityKey _signingKey;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;

        var keyBytes = Convert.FromBase64String(_jwtSettings.SigningKey);

        if (keyBytes.Length < 32)
        {
            throw new ArgumentException("Signing key too short. Pleese use at least 128bits.");
        }

        _signingKey = new SymmetricSecurityKey(keyBytes);
    }

    public string GenerateAccessToken(TokenClaims claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claimsList = new List<Claim>
        {
            new Claim("userId", claims.UserId),
            new Claim(ClaimTypes.Email, claims.Email),
            new Claim(ClaimTypes.Role, claims.Role.ToString()),
            new Claim("fullName", claims.FullName),
            new Claim("tenantId", claims.TenantId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claimsList),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public RefreshToken CreateRefreshToken(Guid userId, string ipAddress, string userAgent)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = GenerateRefreshToken(),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsRevoked = false
        };
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

}