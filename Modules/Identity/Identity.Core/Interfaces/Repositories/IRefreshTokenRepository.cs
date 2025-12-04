using Identity.Core.Entities;
using Common.Core.Interfaces.Repsitories;

namespace Identity.Core.Interfaces.Repositories;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<RefreshToken> AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokensAsync(Guid userId, string reason, CancellationToken cancellationToken = default);
}