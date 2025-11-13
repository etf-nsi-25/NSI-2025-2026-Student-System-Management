using Identity.Core.Interfaces;
using Identity.Infrastructure.Db;
using Identity.Infrastructure.Repositories;


namespace Identity.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly IdentityDbContext _context;
    private IUserRepository? _userRepository;
    private IRefreshTokenRepository? _refreshTokenRepository;

    public UnitOfWork(IdentityDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users
    {
        get
        {
            if (_userRepository == null)
            {
                _userRepository = new UserRepository(_context);
            }
            return _userRepository;
        }
    }

    public IRefreshTokenRepository RefreshTokens
    {
        get
        {
            if (_refreshTokenRepository == null)
            {
                _refreshTokenRepository = new RefreshTokenRepository(_context);
            }
            return _refreshTokenRepository;
        }
    }

    public int Save()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}