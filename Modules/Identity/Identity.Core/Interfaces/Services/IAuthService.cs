using Identity.Core.Models;

namespace Identity.Core.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(string email, string password,  CancellationToken cancellationToken = default);
    Task<AuthResult> RefreshAuthenticationAsync(string refreshToken,  CancellationToken cancellationToken = default);
    Task RevokeAuthenticationAsync(string refreshToken, CancellationToken cancellationToken = default);
}
