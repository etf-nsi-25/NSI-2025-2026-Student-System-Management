using Identity.Core.Entities;
using Identity.Core.Repositories;
using Identity.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Identity.Core.DTO;

namespace Identity.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;

    }

    public async Task<bool> AnySuperAdminExistsAsync(CancellationToken cancellationToken = default)
{
    // TODO: TEAM KILO MIGRATION - This query relies on ApplicationUser.Role.
    // If Team Kilo changes how Roles are stored (e.g., to AspNetRoles table), this query must be updated.
    
    return await _context.Users
        .AnyAsync(u => u.Role == Identity.Core.Enums.UserRole.Superadmin, cancellationToken);
}


    public async Task AddAsync(User user)
    {
        await _context.DomainUsers.AddAsync(user);
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await _context.DomainUsers.FindAsync(userId);
    }

    public async Task<bool> IsUsernameTakenAsync(string username)
    {
        return await _context.DomainUsers.AnyAsync(u => u.Username == username);
    }


    public Task UpdateAsync(User user)
    {
        _context.Set<User>().Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user)
    {
        _context.Set<User>().Remove(user);
        return Task.CompletedTask;
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<User>> GetAllFilteredAsync(UserFilterRequest filter)
    {
        var query = _context.DomainUsers.AsNoTracking();

        if (filter.FacultyId.HasValue)
        {
            query = query.Where(u => u.FacultyId == filter.FacultyId.Value);
        }
        if (filter.Role.HasValue)
        {
            query = query.Where(u => u.Role == filter.Role.Value);
        }


        query = ApplySorting(query, filter.SortBy, filter.SortOrder);

        var skip = (filter.PageNumber - 1) * filter.PageSize;
        query = query.Skip(skip).Take(filter.PageSize);

        return await query.ToListAsync();
    }

    public async Task<int> CountAsync(UserFilterRequest filter)
    {
        var query = _context.DomainUsers.AsNoTracking();

        if (filter.FacultyId.HasValue)
        {
            query = query.Where(u => u.FacultyId == filter.FacultyId.Value);
        }
        if (filter.Role.HasValue)
        {
            query = query.Where(u => u.Role == filter.Role.Value);
        }

        return await query.CountAsync();
    }

    private static IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, string sortOrder)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderBy(u => u.Id);
        }

        bool isAscending = sortOrder.ToLowerInvariant() == "asc";

        IOrderedQueryable<User> orderedQuery;

        orderedQuery = sortBy.ToLowerInvariant() switch
        {
            "username" => isAscending ? query.OrderBy(u => u.Username) : query.OrderByDescending(u => u.Username),
            "firstname" => isAscending ? query.OrderBy(u => u.FirstName) : query.OrderByDescending(u => u.FirstName),
            "role" => isAscending ? query.OrderBy(u => u.Role) : query.OrderByDescending(u => u.Role),
            _ => query.OrderBy(u => u.Id)
        };

        return orderedQuery;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.DomainUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}