using Common.Infrastructure.Repositories;
using Identity.Core.Entities;
using Identity.Core.Interfaces.Repositories;
using Identity.Infrastructure.Db;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly AuthDbContext _context;

public UserRepository(AuthDbContext context) : base(context)
{
    // Pass context to BaseRepository
    _context = context;
}

    public Task<User> CreateUser(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }

        return await FirstOrDefaultAsync(
            u => u.Email == email,
            cancellationToken
        );
    }

    public Task Save()
    {
        throw new NotImplementedException();
    }
}
