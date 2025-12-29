using Identity.Core.Entities;
using Identity.Core.Models;
using System.Security.Claims;

namespace Identity.Core.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(TokenClaims claims);
    string GenerateRefreshToken();
    RefreshToken CreateRefreshToken(Guid userId, string ipAddress, string userAgent);
    ClaimsPrincipal? ValidateAccessToken(string token);
}