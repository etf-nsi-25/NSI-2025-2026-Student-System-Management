using Identity.Core.Entities;

namespace Identity.Core.Repositories;

public interface IUserRepository
{
    public Task<ApplicationUser> CreateUser(string email);

    public Task Save();
}