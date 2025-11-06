using Identity.Core.Entities;
using Identity.Core.Models;
using System.Security.Claims;

namespace Identity.Core.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(TokenClaims claims);
    string GenerateRefreshToken();
    RefreshToken CreateRefreshToken(Guid userId, string ipAddress, string userAgent);
    PublicKeyInfo GetPublicKey();
    ClaimsPrincipal? ValidateAccessToken(string token);
}