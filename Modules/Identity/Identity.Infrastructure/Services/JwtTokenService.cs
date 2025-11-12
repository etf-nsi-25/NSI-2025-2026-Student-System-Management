using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Identity.Core.Entities;
using Identity.Core.Interfaces;
using Identity.Application.DTOs.Auth;
using Identity.Core.Models;


namespace Identity.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly RSA _rsa;
    private readonly RsaSecurityKey _privateKey;
    private readonly RsaSecurityKey _publicKey;
    private readonly string _keyId;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;

        // Initialize RSA keys
        _rsa = RSA.Create(2048);
        _keyId = Guid.NewGuid().ToString();

        // Load keys from configuration or generate new ones
        if (!string.IsNullOrEmpty(_jwtSettings.PrivateKey))
        {
            _rsa.ImportRSAPrivateKey(Convert.FromBase64String(_jwtSettings.PrivateKey), out _);
        }

        _privateKey = new RsaSecurityKey(_rsa) { KeyId = _keyId };
        _publicKey = new RsaSecurityKey(_rsa.ExportParameters(false)) { KeyId = _keyId };
    }

    public string GenerateAccessToken(TokenClaims claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claimsList = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, claims.UserId),
            new Claim(ClaimTypes.Email, claims.Email),
            new Claim(ClaimTypes.Role, claims.Role),
            new Claim(ClaimTypes.Name, claims.FullName),
            new Claim("tenant_id", claims.TenantId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claimsList),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256)
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

    public PublicKeyInfo GetPublicKey()
    {
        var publicKeyBytes = _rsa.ExportRSAPublicKey();
        return new PublicKeyInfo
        {
            PublicKey = Convert.ToBase64String(publicKeyBytes),
            Algorithm = "RS256",
            KeyId = _keyId
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
                IssuerSigningKey = _publicKey,
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }

}