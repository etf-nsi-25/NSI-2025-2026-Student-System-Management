using Identity.Core.Entities;

namespace Identity.Core.Interfaces.Services;

public interface IUserService
{
    public Task<User> CreateUser(string email);
}