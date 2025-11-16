using Common.Core.Interfaces;
using Identity.Core.Entities;

namespace Identity.Core.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    public Task<User> CreateUser(string email);
    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);

    public Task Save();
}