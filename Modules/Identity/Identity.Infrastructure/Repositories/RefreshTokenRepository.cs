using Common.Infrastructure.Repositories;
using Identity.Core.Entities;
using Identity.Core.Interfaces.Repositories;
using Identity.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    private new readonly AuthDbContext _context;

    public RefreshTokenRepository(AuthDbContext context) : base(context)
    {
        // Pass context to BaseRepository
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet // _dbSet comes from BaseRepository
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }
    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await GetAsync(
              filter: rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow,
              orderBy: q => q.OrderByDescending(rt => rt.CreatedAt),
              cancellationToken: cancellationToken
          );
    }


    public async Task RevokeAllUserTokensAsync(Guid userId, string reason, CancellationToken cancellationToken = default)
    {
        var tokens = await GetActiveTokensByUserIdAsync(userId, cancellationToken);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedReason = reason;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
