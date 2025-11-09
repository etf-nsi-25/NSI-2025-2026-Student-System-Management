using Identity.Core.Entities;

namespace Identity.Application.Services;

public interface IUserService
{
    public Task<User> CreateUser(string email);
}