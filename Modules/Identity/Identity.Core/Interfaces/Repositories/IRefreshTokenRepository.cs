using Identity.Core.Entities;
using Common.Core.Interfaces.Repsitories;

namespace Identity.Core.Interfaces.Repositories;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokensAsync(string userId, string reason, CancellationToken cancellationToken = default);
}