using Identity.Core.Entities;
using Identity.Core.DTO;

namespace Identity.Core.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    
    Task<User?> GetByIdAsync(Guid userId);
    Task<bool> IsUsernameTakenAsync(string username); 
    
    Task UpdateAsync(User user);
    
    Task DeleteAsync(User user);
        
    Task SaveAsync();

    Task<IReadOnlyList<User>> GetAllFilteredAsync(UserFilterRequest filter);

    Task<int> CountAsync(UserFilterRequest filter);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    // --- ADDED FOR TASK 598 ---
    /// Checks if any user with the Superadmin role already exists in the database.
    Task<bool> AnySuperAdminExistsAsync(CancellationToken cancellationToken = default);
}